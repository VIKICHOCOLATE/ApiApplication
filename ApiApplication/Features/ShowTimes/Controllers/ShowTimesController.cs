using ApiApplication.Database.Entities;
using ApiApplication.Features.ShowTimes.DTOs;
using ApiApplication.Shared.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiApplication.Features.ShowTimes.Controllers
{
    [ApiController]
    public class ShowTimesController : ControllerBase
    {
        private readonly IShowtimesService _showTimesService;
		private readonly IMovieService _externalMovieService;
		private readonly IMapper _mapper;
		public ShowTimesController(IShowtimesService showTimesService, 
			IMovieService externalMovieService,
			IMapper mapper)
        {
            _showTimesService = showTimesService ?? throw new ArgumentNullException(nameof(showTimesService));
			_externalMovieService = externalMovieService ?? throw new ArgumentNullException(nameof(externalMovieService));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
		}

		/// <summary>
		/// Creates a showtime for a movie given its external ID.
		/// </summary>
		/// <param name="showtimeDto">The details for creating the showtime.</param>
		/// <returns>The created showtime or an error message.</returns>
		[HttpPost("api/showtimes")]
        public async Task<ActionResult<ShowtimeDTO>> CreateShowtimeAsync([FromBody] ShowtimeDTO showtimeDto)
        {
			var externalMovieResult = await FetchMovieFromExternalService(showtimeDto.ExternalMovieId);
			if (!externalMovieResult.IsSuccess)
			{
				return BadRequest($"Could not fetch movie details. Error: {externalMovieResult.ErrorMessage}");
			}

			var showtimeCreationResult = await CreateShowtimeWithMovie(externalMovieResult.Movie, showtimeDto);
			if (!showtimeCreationResult.IsSuccess)
			{
				return StatusCode(500, new
				{
					status = 500,
					message = showtimeCreationResult.ErrorMessage
				});
			}

			// Return the created resource with a 201 status code, and set the location header
			return Created(string.Empty, showtimeCreationResult.ShowTime);
		}

		private async Task<(bool IsSuccess, MovieEntity Movie, string ErrorMessage)> FetchMovieFromExternalService(string externalMovieId)
		{
			var (isSuccess, externalMovie, errorMessage) = await _externalMovieService.GetByIdAsync(externalMovieId.ToString());
			if (!isSuccess)
			{
				return (false, null, errorMessage);
			}

			var movieEntity = _mapper.Map<MovieEntity>(externalMovie);
			return (true, movieEntity, null);
		}

		private async Task<(bool IsSuccess, ShowtimeDTO ShowTime, string ErrorMessage)> CreateShowtimeWithMovie(MovieEntity movie, ShowtimeDTO showtimeDto)
		{
			var showtimeEntity = new ShowtimeEntity
			{
				Id = showtimeDto.Id,
				Movie = movie,
				SessionDate = showtimeDto.ShowtimeDate,
				AuditoriumId = showtimeDto.AuditoriumId,
				Tickets = new List<TicketEntity>()  // Initialize an empty list or add default ticket entries if needed
			};

			return await _showTimesService.CreateShowtimeWithMovieAsync(showtimeEntity);
		}
	}
}
