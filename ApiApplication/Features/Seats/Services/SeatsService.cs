using ApiApplication.Database.Entities;
using ApiApplication.Database.Repositories.Abstractions;
using ApiApplication.Models;
using ApiApplication.Shared.Interfaces;
using ApiApplication.Shared.Utilities;
using Microsoft.EntityFrameworkCore;
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

        public SeatsService(ITicketsRepository ticketsRepository, IShowtimesRepository showtimesRepository)
        {
            _ticketsRepository = ticketsRepository;
            _showtimesRepository = showtimesRepository;
        }

        public async Task<ReservationResponse> ReserveSeats(int showtimeId, List<SeatEntity> desiredSeats)
        {
            using var transaction = _ticketsRepository.BeginTransaction();

            try
            {
                ValidateContiguousSeats(desiredSeats);
                await EnsureSeatsAreAvailable(showtimeId, desiredSeats);

                var showtime = await _showtimesRepository.GetAllAsync(s => s.Id == showtimeId, CancellationToken.None);
                if (showtime == null || !showtime.Any()) throw new InvalidOperationException(ErrorMessages.Seats.ShowtimeNotFound);

                var reservedTicket = await _ticketsRepository.CreateAsync(showtime.First(), desiredSeats, CancellationToken.None);

                return new ReservationResponse
                {
                    ReservationGuid = reservedTicket.Id.ToString(),
                    NumberOfSeats = desiredSeats.Count,
                    AuditoriumId = desiredSeats[0].AuditoriumId,
                    MovieTitle = showtime.First().Movie.Title
                };
            }
            catch (DbUpdateConcurrencyException)
            {
                _ticketsRepository.Rollback();
                throw new InvalidOperationException(ErrorMessages.Seats.ConcurrencyConflictError);
            }

        }

        public async Task BuySeat(Guid reservationGuid)
        {
            using var transaction = _ticketsRepository.BeginTransaction();

            try
            {
                var ticket = await _ticketsRepository.GetAsync(reservationGuid, CancellationToken.None);
                if (ticket == null)
                {
                    throw new InvalidOperationException(ErrorMessages.Seats.InvalidReservation);
                }
                await _ticketsRepository.ConfirmPaymentAsync(ticket, CancellationToken.None);

                _ticketsRepository.Commit();
            }
            catch (DbUpdateConcurrencyException)
            {
                _ticketsRepository.Rollback();
                throw new InvalidOperationException(ErrorMessages.Seats.ConcurrencyConflictError);
            }
        }

        private void ValidateContiguousSeats(List<SeatEntity> desiredSeats)
        {
            var sortedSeats = desiredSeats.OrderBy(s => s.Row).ThenBy(s => s.SeatNumber).ToList();
            for (int i = 1; i < sortedSeats.Count; i++)
            {
                if (sortedSeats[i].Row == sortedSeats[i - 1].Row && sortedSeats[i].SeatNumber - sortedSeats[i - 1].SeatNumber != 1)
                {
                    throw new InvalidOperationException("Seats are not contiguous.");
                }
            }
        }

        private async Task EnsureSeatsAreAvailable(int showtimeId, List<SeatEntity> desiredSeats)
        {
            var existingTickets = await _ticketsRepository.GetEnrichedAsync(showtimeId, CancellationToken.None);
            var reservedSeats = existingTickets.SelectMany(t => t.Seats).ToList();

            foreach (var seat in desiredSeats)
            {
                if (reservedSeats.Any(r => r.Row == seat.Row && r.SeatNumber == seat.SeatNumber))
                {
                    throw new InvalidOperationException("One or more seats are already reserved or sold.");
                }
            }
        }
    }
}
