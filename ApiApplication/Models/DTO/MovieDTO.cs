using System; 

namespace ApiApplication.Models.DTO
{
    public class MovieDTO
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string ImdbId { get; set; }
        public string Stars { get; set; }
        public DateTime ReleaseDate { get; set; }
    }
}