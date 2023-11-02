using ApiApplication.Database.Entities;
using ApiApplication.Database.Repositories.Abstractions;
using ApiApplication.Interfaces;
using ApiApplication.Models.DTO;
using ApiApplication.Services;
using AutoMapper;

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ApiApplication.Providers
{
    public class ShowtimesService
    {
		private readonly IShowtimesRepository _showtimesRepository;
		private readonly IExternalMovieService _externalMovieService;
		private readonly IMapper _mapper;

		public ShowtimesService(IShowtimesRepository showtimesRepository, ExternalMovieService externalMovieService, IMapper mapper)
		{
			_showtimesRepository = showtimesRepository ?? throw new ArgumentNullException(nameof(showtimesRepository));
			_externalMovieService = externalMovieService ?? throw new ArgumentNullException(nameof(externalMovieService));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
		}

		public async Task<IEnumerable<ShowtimeDTO>> GetAllShowTimesAsync(Expression<Func<ShowtimeEntity, bool>> filter = null, CancellationToken cancel = default)
        {
            var showTimes = await _showtimesRepository.GetAllAsync(filter, cancel);
			return _mapper.Map<IEnumerable<ShowtimeDTO>>(showTimes);
        }

		public async Task<ShowtimeDTO> CreateShowtimeWithMovieAsync(string externalMovieId, CancellationToken cancellationToken = default)
		{
			var movieData = await _externalMovieService.FetchMovieByIdAsync(externalMovieId);
			if (movieData == null)
			{
				throw new InvalidOperationException("Could not fetch movie data.");
			}

			var movieEntity = _mapper.Map<MovieEntity>(movieData);
			var showtimeEntity = GenerateShowtimeEntity(movieEntity);
			var createdShowtime = await _showtimesRepository.CreateShowtime(showtimeEntity, cancellationToken);

			return _mapper.Map<ShowtimeDTO>(createdShowtime);
		}

		private ShowtimeEntity GenerateShowtimeEntity(MovieEntity movieEntity)
		{
			return new ShowtimeEntity()
			{
				Id = GetRandomInt(),
				AuditoriumId = GetRandomInt(),
				Movie = movieEntity,
				SessionDate = DateTime.Now,
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