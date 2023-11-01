using ApiApplication.Interfaces;
using ApiApplication.Models.DTO;

using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApiApplication.Services
{
    public class ExternalMovieService : IExternalMovieService
	{
        private readonly IHttpClientFactory _httpClientFactory;
		private const string BasePath = "v1/movies/";


		public ExternalMovieService(IHttpClientFactory httpClientFactory)
        {
			_httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
		}

        public async Task<ExternalMovieDTO> FetchMovieByIdAsync(string movieId)
        {
			try
			{
				var client = _httpClientFactory.CreateClient("ExternalMovies");
				var response = await client.GetAsync($"{BasePath}{movieId}");

				if (!response.IsSuccessStatusCode)
				{
					// Log or handle based on status code
					return null;
				}

				var content = await response.Content.ReadAsStringAsync();
				var options = new JsonSerializerOptions
				{
					PropertyNameCaseInsensitive = true
				};

				return JsonSerializer.Deserialize<ExternalMovieDTO>(content, options);
			}
			catch (Exception ex)
			{
				// Log the exception
				return null;
			}
			
        }
    }
}
