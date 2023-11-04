using ApiApplication.Features.Seats.DTOs;
using ApiApplication.Shared.Interfaces;
using ApiApplication.Shared.Utilities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiApplication.Features.Seats.Controllers
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
		/// reserve seats for a showtime
		/// </summary>
		/// <param name="showtimeId"></param>
		/// <param name="desiredSeats"></param>
		/// <returns></returns>
		[HttpPost("reservations")]
        public async Task<IActionResult> ReserveSeats(int showtimeId, [FromBody] List<SeatDto> desiredSeats)
        {
            if (desiredSeats == null || !desiredSeats.Any())
            {
                return BadRequest(ErrorMessages.Seats.EmptySeatsError);
            }

            try
            {
                var result = await _seatsService.ReserveSeats(showtimeId, desiredSeats);
				if (result.IsSuccess)
					return Ok(result.ReservationResponse);

				return StatusCode(500, new
				{
					status = 500,
					message = result.ErrorMessage
				});
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
                return Ok(ErrorMessages.Seats.PurchaseConfirmation);
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
            return StatusCode(500, $"An error {ex.Message} occurred while processing your request.");
        }
    }
}
