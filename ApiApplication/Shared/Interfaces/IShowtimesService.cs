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
        Task<(bool IsSuccess, ShowtimeDTO ShowTime, string ErrorMessage)> CreateShowtimeWithMovieAsync(ShowtimeEntity showtimeEntity, CancellationToken cancellationToken = default);
    }
}
