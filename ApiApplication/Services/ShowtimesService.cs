using ApiApplication.Database.Entities;
using ApiApplication.Database.Repositories.Abstractions;
using ApiApplication.Models.DTO;
using ApiApplication.Shared.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ApiApplication.Providers
{
    public class ShowtimesService : IShowtimesService
	{
		private readonly IShowtimesRepository _showtimesRepository;
		private readonly IExternalMovieService _externalMovieService;
		private readonly IMapper _mapper;

		private readonly ILogger<ShowtimesService> _logger;

		public ShowtimesService(IShowtimesRepository showtimesRepository, 
								IExternalMovieService externalMovieService, 
								IMapper mapper, 
								ILogger<ShowtimesService> logger)
		{
			_showtimesRepository = showtimesRepository ?? throw new ArgumentNullException(nameof(showtimesRepository));
			_externalMovieService = externalMovieService ?? throw new ArgumentNullException(nameof(externalMovieService));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<IEnumerable<ShowtimeDTO>> GetAllShowTimesAsync(Expression<Func<ShowtimeEntity, bool>> filter = null, CancellationToken cancel = default)
        {
            var showTimes = await _showtimesRepository.GetAllAsync(filter, cancel);
			return _mapper.Map<IEnumerable<ShowtimeDTO>>(showTimes);
        }

		public async Task<(bool IsSuccess, ShowtimeDTO ShowTime, string ErrorMessage)> CreateShowtimeWithMovieAsync(string externalMovieId, CancellationToken cancellationToken = default)
		{
			try
			{
				var result = await _externalMovieService.FetchMovieByIdAsync(externalMovieId);
				if (result.IsSuccess)
				{
					var movieEntity = _mapper.Map<MovieEntity>(result.Movie);
					var showtimeEntity = GenerateShowtimeEntity(movieEntity);

					var createdShowtime = await _showtimesRepository.CreateShowtime(showtimeEntity, cancellationToken);
					var createdShowtimeResult = _mapper.Map<ShowtimeDTO>(createdShowtime);
					return (true, createdShowtimeResult, null);
				}
				return (false, null, result.ErrorMessage);
				
			}
			catch (Exception ex)
			{
				_logger?.LogError(ex.ToString());
				return (false, null, ex.Message);
			}
		}

		private ShowtimeEntity GenerateShowtimeEntity(MovieEntity movieEntity)
		{
			return new ShowtimeEntity()
			{
				Id = GetRandomInt(),
				AuditoriumId = GetRandomInt(),
				Movie = movieEntity,
				SessionDate = DateTime.UtcNow,
				Tickets = GetTickets()
			};
		}

		private int GetRandomInt()
		{
			Random random = new Random();
			return random.Next();
		}

		private List<TicketEntity> GetTickets()
		{
			return new List<TicketEntity>() { new TicketEntity() };
		}
	}
}