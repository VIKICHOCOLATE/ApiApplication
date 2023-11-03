using System;

namespace ApiApplication.Models.DTO
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
