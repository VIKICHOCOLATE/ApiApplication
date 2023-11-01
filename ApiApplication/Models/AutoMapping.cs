using ApiApplication.Database.Entities;
using ApiApplication.Models.DTO;

using AutoMapper;

namespace ApiApplication.Models
{
    public class AutoMapping: Profile
    {
        public AutoMapping()
        {
            CreateMap<MovieEntity, MovieDTO>();

            CreateMap<ShowtimeEntity, ShowtimeDTO>()
            .ForMember(dest => dest.MovieId, opt => opt.MapFrom(src => src.Movie.Id))
            .ForMember(dest => dest.MovieTitle, opt => opt.MapFrom(src => src.Movie.Title));
        }
    }
}
