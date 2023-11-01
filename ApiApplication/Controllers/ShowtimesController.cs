using ApiApplication.Providers;

using Microsoft.AspNetCore.Mvc;

using System.Threading.Tasks;

namespace ApiApplication.Controllers
{
    [ApiController]
    public class ShowTimesController : ControllerBase
    {
        private readonly ShowtimesService _showTimesService;
        public ShowTimesController(ShowtimesService showTimesService)
        {
            _showTimesService = showTimesService;
        }

        [HttpPost]
        [Route("showtime")]
        public async Task<IActionResult> CreateShowtimeAsync(string externalMovieId)
        {
            var createdShowtime = await _showTimesService.CreateShowtimeWithMovieAsync(externalMovieId);

            if (createdShowtime == null)
                return BadRequest("Failed to create showtime.");

            return Ok(createdShowtime);
        }
    }
}
