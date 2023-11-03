using ApiApplication.Features.Movies.DTOs;
using ApiApplication.Shared.Interfaces;
using ApiApplication.Shared.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApiApplication.Features.Movies.Services
{
	public class ExternalMovieService : IExternalMovieService
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly ILogger<ExternalMovieService> _logger;
		private readonly string _apiKey;

		private const string BasePath = "v1/movies/";
		private static readonly string ApiKeyConfigurationPath = "Services:ExternalMovies:ApiKey";
		private static readonly string ApiKeyHeaderName = "X-Apikey";

		private readonly ICacheService _cacheService;

		public ExternalMovieService(IHttpClientFactory httpClientFactory,
			IConfiguration configuration,
			ILogger<ExternalMovieService> logger,
			ICacheService cacheService)
		{
			_httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));

			_apiKey = configuration[ApiKeyConfigurationPath];
			_cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));

		}

		public async Task<(bool IsSuccess, ExternalMovieDTO Movie, string ErrorMessage)> FetchMovieByIdAsync(string movieId)
		{
			var cacheKey = $"Movie_{movieId}";

			// First, try to get the movie data from the cache
			var cachedMovie = await _cacheService.GetCachedDataAsync<ExternalMovieDTO>(cacheKey);

			if (cachedMovie != null)
			{
				return (true, cachedMovie, null);
			}

			try
			{
				using var client = _httpClientFactory.CreateClient("ExternalMovies");
				client.DefaultRequestHeaders.Add(ApiKeyHeaderName, _apiKey);

				var response = await client.GetAsync($"{BasePath}{movieId}");

				if (response.IsSuccessStatusCode)
				{
					var content = await response.Content.ReadAsStringAsync();
					var options = new JsonSerializerOptions
					{
						PropertyNameCaseInsensitive = true
					};
					var result = JsonSerializer.Deserialize<ExternalMovieDTO>(content, options);

					// Cache the fetched movie data
					await _cacheService.SetCacheDataAsync(cacheKey, result, TimeSpan.FromHours(1)); // 1 hour cache time


					return (true, result, null);
				}
				return (false, null, response.ReasonPhrase);
			}
			catch (Exception ex)
			{
				_logger?.LogError(ex.ToString());
				return (false, null, ex.Message);
			}
		}
	}
}