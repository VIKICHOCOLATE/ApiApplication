using ApiApplication.Database.Entities;
using ApiApplication.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiApplication.Shared.Interfaces
{
    public interface ISeatsService
    {
        Task<ReservationResponse> ReserveSeats(int showtimeId, List<SeatEntity> desiredSeats);
        Task BuySeat(Guid reservationGuid);
    }
}
