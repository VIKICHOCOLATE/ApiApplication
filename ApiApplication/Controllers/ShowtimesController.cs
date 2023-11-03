using ApiApplication.Models.DTO;
using ApiApplication.Shared.Interfaces;
using ApiApplication.Shared.Utilities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ApiApplication.Controllers
{
    [ApiController]
	public class ShowTimesController : ControllerBase
    {
        private readonly IShowtimesService _showTimesService;
        public ShowTimesController(IShowtimesService showTimesService)
        {
			_showTimesService = showTimesService ?? throw new ArgumentNullException(nameof(showTimesService));
		}

		/// <summary>
		/// Creates a showtime for a movie given its external ID.
		/// </summary>
		/// <param name="externalMovieId">The external ID of the movie.</param>
		/// <returns>The created showtime or an error message.</returns>
		[HttpPost("api/showtimes")]
		public async Task<ActionResult<ShowtimeDTO>> CreateShowtimeAsync([FromBody] string externalMovieId)
        {
			if (string.IsNullOrWhiteSpace(externalMovieId))
				return BadRequest(ErrorMessages.ShowTimes.EmptyExternalMovieId);

			var result = await _showTimesService.CreateShowtimeWithMovieAsync(externalMovieId);

			if (result.IsSuccess)
				return Ok(result.ShowTime);

			return StatusCode(500, new
			{
				status = 500,
				message = result.ErrorMessage
			});
		}
	}
}
