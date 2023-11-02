using ApiApplication.Interfaces;

using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ApiApplication.Controllers
{
    [ApiController]
	[Route("api/[controller]")]
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
		[HttpPost]
        public async Task<IActionResult> CreateShowtimeAsync([FromBody] string externalMovieId)
        {
			if (string.IsNullOrWhiteSpace(externalMovieId))
				return BadRequest("External movie ID cannot be empty or null.");

			var result = await _showTimesService.CreateShowtimeWithMovieAsync(externalMovieId);

			if (result.IsSuccess)
				return Ok(result.ShowTime);

            return new ObjectResult(new
			{
				status = 500,
				message = result.ErrorMessage
			})
			{
				StatusCode = 500
			};
		}
	}
}
