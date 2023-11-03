﻿using ApiApplication.Database.Entities;
using ApiApplication.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiApplication.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class SeatsController : ControllerBase
	{
		private readonly ISeatsService _seatsService;

		public SeatsController(ISeatsService seatsService)
		{
			_seatsService = seatsService ?? throw new ArgumentNullException(nameof(seatsService));
		}

		/// <summary>
		/// Reserve seats for a movie
		/// </summary>
		/// <param name="seatIds">List of seat IDs to be reserved</param>
		/// <param name="movieTitle">Title of the movie for which the seats are being reserved</param>
		/// <returns>Reservation details or error message</returns>
		[HttpPost("reservations")]
		public async Task<IActionResult> ReserveSeats(int showtimeId, [FromBody] List<SeatEntity> desiredSeats)
		{
			if (desiredSeats == null || !desiredSeats.Any())
			{
				return BadRequest("Seats list cannot be empty.");
			}

			try
			{
				var reservationResponse = await _seatsService.ReserveSeats(showtimeId, desiredSeats);
				return Ok(reservationResponse);
			}
			catch (InvalidOperationException ex)
			{
				return HandleKnownException(ex);
			}
			catch (Exception ex)
			{
				return HandleUnknownException(ex);
			}
		}

		[HttpPut("reservations/{reservationGuid}/purchase")]
		public async Task<IActionResult> BuySeat([FromBody] Guid reservationGuid)
		{
			try
			{
				await _seatsService.BuySeat(reservationGuid);
				return Ok("Seat successfully purchased.");
			}
			catch (InvalidOperationException ex)
			{
				return HandleKnownException(ex);
			}
			catch (Exception ex)
			{
				return HandleUnknownException(ex);
			}
		}

		private IActionResult HandleKnownException(Exception ex)
		{
			return BadRequest(ex.Message);
		}

		private IActionResult HandleUnknownException(Exception ex)
		{
			return StatusCode(500, "An error occurred while processing your request.");
		}
	}
}
