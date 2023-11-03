using ApiApplication.Features.Movies.DTOs;
using System.Threading.Tasks;

namespace ApiApplication.Shared.Interfaces
{
    public interface IExternalMovieService
    {
        Task<(bool IsSuccess, ExternalMovieDTO Movie, string ErrorMessage)> FetchMovieByIdAsync(string movieId);
		Task<(bool IsSuccess, ExternalMovieListDTO Movies, string ErrorMessage)> FetchAllMoviesAsync();

	}
}
