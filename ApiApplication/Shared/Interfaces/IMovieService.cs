using ApiApplication.Features.Movies.DTOs;
using System.Threading.Tasks;

namespace ApiApplication.Shared.Interfaces
{
    public interface IMovieService
    {
        Task<(bool IsSuccess, ExternalMovieDTO Movie, string ErrorMessage)> GetByIdAsync(string movieId);
		Task<(bool IsSuccess, ExternalMovieListDTO Movies, string ErrorMessage)> GetAllAsync();

	}
}
