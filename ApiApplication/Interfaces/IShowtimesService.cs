using ApiApplication.Database.Entities;
using ApiApplication.Models.DTO;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace ApiApplication.Interfaces
{
	public interface IShowtimesService
	{
		Task<IEnumerable<ShowtimeDTO>> GetAllShowTimesAsync(Expression<Func<ShowtimeEntity, bool>> filter = null, CancellationToken cancel = default);
		Task<(bool IsSuccess, ShowtimeDTO ShowTime, string ErrorMessage)> CreateShowtimeWithMovieAsync(string externalMovieId, CancellationToken cancellationToken = default);
	}
}
