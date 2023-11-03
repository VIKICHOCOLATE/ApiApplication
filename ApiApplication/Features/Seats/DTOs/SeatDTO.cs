using System;
using ApiApplication.Features.Seats.Models;

namespace ApiApplication.Features.Seats.DTOs
{
    public class SeatDTO
    {
        public int SeatId { get; set; }
        public int AuditoriumId { get; set; }
        public SeatStatus Status { get; set; }
        public DateTime? LastReservedAt { get; set; }
        public string ReservationGuid { get; set; }
    }
}
