using ApiApplication.Database.Entities;
using System;

namespace ApiApplication.Features.ShowTimes.DTOs
{
    public class ShowtimeDTO
    {
        public int Id { get; set; }
        public string MovieId { get; set; }
        public string MovieTitle { get; set; }
        public DateTime SessionDate { get; set; }
        public int AuditoriumId { get; set; }
    }
}
