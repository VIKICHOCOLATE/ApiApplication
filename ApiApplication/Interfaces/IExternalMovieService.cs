using ApiApplication.Models.DTO;
using System.Threading.Tasks;

namespace ApiApplication.Interfaces
{
	public interface IExternalMovieService
	{
		Task<ExternalMovieDTO> FetchMovieByIdAsync(string movieId);
	}
}
