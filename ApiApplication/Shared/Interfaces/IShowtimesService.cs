using ApiApplication.Database.Entities;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Threading;
using System;
using ApiApplication.Features.ShowTimes.DTOs;

namespace ApiApplication.Shared.Interfaces
{
    public interface IShowtimesService
    {
        Task<IEnumerable<ShowtimeDTO>> GetAllShowTimesAsync(Expression<Func<ShowtimeEntity, bool>> filter = null, CancellationToken cancel = default);
        Task<(bool IsSuccess, ShowtimeDTO ShowTime, string ErrorMessage)> CreateShowtimeWithMovieAsync(string externalMovieId, CancellationToken cancellationToken = default);
    }
}
