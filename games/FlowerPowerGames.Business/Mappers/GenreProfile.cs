using AutoMapper;
using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Data.Models;

namespace FlowerPowerGames.Business.Mappers;

public class GenreProfile : Profile
{

    public GenreProfile ()
    {
        CreateMap<GenreProfile, GenreDto>();

        CreateMap<GenreDto, Genre>()
            .ForMember(
                dest => dest.Id,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.Games,
                opt => opt.Ignore());
    }

}
