using ApiApplication.Models.DTO;
using System.Threading.Tasks;

namespace ApiApplication.Interfaces
{
	public interface IExternalMovieService
	{
		Task<(bool IsSuccess, ExternalMovieDTO Movie, string ErrorMessage)> FetchMovieByIdAsync(string movieId);
	}
}
