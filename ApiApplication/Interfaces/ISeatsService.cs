using ApiApplication.Database.Entities;
using ApiApplication.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiApplication.Interfaces
{
	public interface ISeatsService
	{
		Task<ReservationResponse> ReserveSeats(int showtimeId, List<SeatEntity> desiredSeats);
		Task BuySeat(Guid reservationGuid);
	}
}
