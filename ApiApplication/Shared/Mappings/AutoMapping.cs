﻿using ApiApplication.Database.Entities;
using ApiApplication.Features.ShowTimes.DTOs;
using ApiApplication.Models.DTO;

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
            .ForMember(dest => dest.MovieId, opt => opt.MapFrom(src => src.Movie.Id))
            .ForMember(dest => dest.MovieTitle, opt => opt.MapFrom(src => src.Movie.Title));
        }
    }
}
