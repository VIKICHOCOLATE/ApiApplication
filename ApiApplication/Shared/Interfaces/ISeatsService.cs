using ApiApplication.Features.Seats.DTOs;
using ApiApplication.Features.Seats.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiApplication.Shared.Interfaces
{
    public interface ISeatsService
    {
		Task<(bool IsSuccess, ReservationResponse ReservationResponse, string ErrorMessage)> ReserveSeats(int showtimeId, List<SeatDTO> desiredSeats);

		Task BuySeat(Guid reservationGuid);
    }
}
