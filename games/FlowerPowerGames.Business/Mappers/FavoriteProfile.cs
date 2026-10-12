using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Data.Models;

namespace FlowerPowerGames.Business.Mappers;
    
[ExcludeFromCodeCoverage]

public class FavoriteProfile : Profile
{
    public FavoriteProfile()
    {
        CreateMap<Favorite, FavoriteDto>();
        CreateMap<FavoriteDto, Favorite>()
            .ForMember(
                dest => dest.Id,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.Game,
                opt => opt.Ignore());

        CreateMap<CreateFavoriteDto, Favorite>()
            .ForMember(
                dest => dest.Id,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.Game,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.UserId,
                opt => opt.Ignore());

    }
}
