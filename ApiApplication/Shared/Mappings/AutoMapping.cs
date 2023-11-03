using ApiApplication.Database.Entities;
using ApiApplication.Features.Movies.DTOs;
using ApiApplication.Features.Seats.DTOs;
using ApiApplication.Features.ShowTimes.DTOs;

using AutoMapper;

namespace ApiApplication.Shared.Mappings
{
	public class AutoMapping : Profile
	{
		public AutoMapping()
		{
			CreateMap<ExternalMovieDTO, MovieEntity>()
			.ForMember(dest => dest.Stars, opt => opt.MapFrom(src => src.Crew));

			CreateMap<ShowtimeEntity, ShowtimeDTO>()
			.ForMember(dest => dest.ExternalMovieId, opt => opt.MapFrom(src => src.Movie.Id))
			.ForMember(dest => dest.ShowtimeDate, opt => opt.MapFrom(src => src.SessionDate))
			.ForMember(dest => dest.MovieTitle, opt => opt.MapFrom(src => src.Movie.Title));

			CreateMap<SeatDTO, SeatEntity>();
			CreateMap<TicketDTO, TicketEntity>();
			CreateMap<TicketEntity, TicketDTO>();

			CreateMap<ExternalMovieDTO, MovieEntity>();
		}
	}
}