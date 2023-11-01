using ApiApplication.Database.Entities;
using System;

namespace ApiApplication.Models.DTO
{
    public class ShowtimeDTO
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public string MovieTitle { get; set; }
        public DateTime SessionDate { get; set; }
        public int AuditoriumId { get; set; }
    }
}
