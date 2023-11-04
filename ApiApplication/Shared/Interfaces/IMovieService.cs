using ApiApplication.Features.Movies.DTOs;
using System.Threading.Tasks;

namespace ApiApplication.Shared.Interfaces
{
    public interface IMovieService
    {
        Task<(bool IsSuccess, ExternalMovieDto Movie, string ErrorMessage)> GetByIdAsync(string movieId);
		Task<(bool IsSuccess, ExternalMovieListDto Movies, string ErrorMessage)> GetAllAsync();

	}
}
