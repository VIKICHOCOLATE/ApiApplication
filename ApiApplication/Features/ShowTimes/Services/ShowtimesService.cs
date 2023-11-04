using ApiApplication.Database.Entities;
using ApiApplication.Database.Repositories.Abstractions;
using ApiApplication.Features.ShowTimes.DTOs;
using ApiApplication.Shared.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ApiApplication.Features.ShowTimes.Services
{
    public class ShowtimesService : IShowtimesService
    {
        private readonly IShowtimesRepository _showtimesRepository;
        private readonly IMapper _mapper;

        private readonly ILogger<ShowtimesService> _logger;

        public ShowtimesService(IShowtimesRepository showtimesRepository,
                                IMapper mapper,
                                ILogger<ShowtimesService> logger)
        {
            _showtimesRepository = showtimesRepository ?? throw new ArgumentNullException(nameof(showtimesRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<(bool IsSuccess, ShowtimeDto ShowTime, string ErrorMessage)> CreateShowtimeWithMovieAsync(ShowtimeEntity showtimeEntity, CancellationToken cancellationToken = default)
        {
            try
            {
				var createdShowtime = await _showtimesRepository.CreateShowtime(showtimeEntity, CancellationToken.None);
				var createdShowtimeDto = _mapper.Map<ShowtimeDto>(createdShowtime);

				return (true, createdShowtimeDto, null);

            }
            catch (Exception ex)
            {
                _logger?.LogError(ex.ToString());
                return (false, null, ex.Message);
            }
        }
    }
}