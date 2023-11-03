using System;

namespace ApiApplication.Features.ShowTimes.DTOs
{
    public class ShowtimeDTO
    {
        public int Id { get; set; }
        public string ExternalMovieId { get; set; }
        public string MovieTitle { get; set; }
        public DateTime ShowtimeDate { get; set; }
        public int AuditoriumId { get; set; }
    }
}
