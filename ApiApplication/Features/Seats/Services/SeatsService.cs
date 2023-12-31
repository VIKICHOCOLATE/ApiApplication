﻿using ApiApplication.Database.Entities;
using ApiApplication.Database.Repositories.Abstractions;
using ApiApplication.Features.Seats.DTOs;
using ApiApplication.Features.Seats.Models;
using ApiApplication.Shared.Interfaces;
using ApiApplication.Shared.Utilities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ApiApplication.Features.Seats.Services
{
	public class SeatsService : ISeatsService
	{
		private readonly ITicketsRepository _ticketsRepository;
		private readonly IShowtimesRepository _showtimesRepository;

		private readonly IMapper _mapper;
		private readonly ILogger<SeatsService> _logger;

		public SeatsService(ITicketsRepository ticketsRepository, IShowtimesRepository showtimesRepository, IMapper mapper, ILogger<SeatsService> logger)
		{
			_ticketsRepository = ticketsRepository;
			_showtimesRepository = showtimesRepository;

			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<(bool IsSuccess, ReservationResponse ReservationResponse, string ErrorMessage)> ReserveSeats(int showtimeId, List<SeatDto> desiredSeats)
		{
			using var transaction = _ticketsRepository.BeginTransaction();

			try
			{
				bool isContiguous = ValidateContiguousSeats(desiredSeats);
				if (!isContiguous)
				{
					return (false, null, ErrorMessages.Seats.SeatsNotContiguousError);
				}
				bool isAvaliable = await EnsureSeatsAreAvailable(showtimeId, desiredSeats);
				if (!isAvaliable)
				{
					return (false, null, ErrorMessages.Seats.SeatsAreSoldError);
				}

				var showtime = await _showtimesRepository.GetAllAsync(s => s.Id == showtimeId, CancellationToken.None);
				if (showtime == null || !showtime.Any())
				{
					return (false, null, ErrorMessages.Seats.ShowtimeNotFound);
				}

				var desiredSeatsEntity = _mapper.Map<List<SeatEntity>>(desiredSeats);
				var reservedTicket = await _ticketsRepository.CreateAsync(showtime.First(), desiredSeatsEntity, CancellationToken.None);

				var reservationResponse = new ReservationResponse
				{
					ReservationGuid = reservedTicket.Id.ToString(),
					NumberOfSeats = desiredSeats.Count,
					AuditoriumId = desiredSeats[0].AuditoriumId,
					MovieTitle = showtime.First().Movie.Title
				};

				return (true, reservationResponse, null);
			}
			catch (DbUpdateConcurrencyException ex)
			{
				_ticketsRepository.Rollback();
				_logger?.LogError(ex.ToString());
				return (false, null, ErrorMessages.Seats.ConcurrencyConflictError);
			}
			catch (Exception ex)
			{
				_ticketsRepository.Rollback();
				_logger?.LogError(ex.ToString());
				return (false, null, ex.Message);
			}
		}

		public async Task<(bool IsSuccess, TicketDTO ticket, string ErrorMessage)> BuySeat(Guid reservationGuid)
		{
			using var transaction = _ticketsRepository.BeginTransaction();

			try
			{
				var ticket = await _ticketsRepository.GetAsync(reservationGuid, CancellationToken.None);
				if (ticket == null)
				{
					return (false, null, ErrorMessages.Seats.InvalidReservation);
				}

				var result = await _ticketsRepository.ConfirmPaymentAsync(ticket, CancellationToken.None);
				_ticketsRepository.Commit();

				var ticketDto = _mapper.Map<TicketDTO>(result);
				return (true, ticketDto, null);
			}
			catch (DbUpdateConcurrencyException ex)
			{
				_ticketsRepository.Rollback();
				_logger?.LogError(ex.ToString());
				return (false, null, ErrorMessages.Seats.ConcurrencyConflictError);
			}
		}

		private bool ValidateContiguousSeats(IEnumerable<SeatDto> desiredSeats)
		{
			var sortedSeats = desiredSeats.OrderBy(s => s.Row).ThenBy(s => s.SeatNumber).ToList();
			for (int i = 1; i < sortedSeats.Count; i++)
			{
				if (sortedSeats[i].Row == sortedSeats[i - 1].Row && sortedSeats[i].SeatNumber - sortedSeats[i - 1].SeatNumber != 1)
				{
					return false;
				}
			}
			return true;
		}

		private async Task<bool> EnsureSeatsAreAvailable(int showtimeId, List<SeatDto> desiredSeats)
		{
			var existingTickets = await _ticketsRepository.GetEnrichedAsync(showtimeId, CancellationToken.None);
			var reservedSeats = existingTickets.SelectMany(t => t.Seats).ToList();

			foreach (var seat in desiredSeats)
			{
				if (reservedSeats.Any(r => r.Row == seat.Row && r.SeatNumber == seat.SeatNumber))
				{
					return false;
				}
			}
			return true;
		}
	}
}
