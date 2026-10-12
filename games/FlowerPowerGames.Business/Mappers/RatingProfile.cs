using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Data.Models;

namespace FlowerPowerGames.Business.Mappers;
    
[ExcludeFromCodeCoverage]

public class RatingProfile : Profile
{
    public RatingProfile()
    {
        CreateMap<Rating, RatingDto>();

        CreateMap<RatingDto, Rating>()
            .ForMember(
                dest => dest.Id,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.Game,
                opt => opt.Ignore());

        CreateMap<CreateRatingDto, Rating>()
            .ForMember(
                dest => dest.Id,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.Game,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.UserId,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.User,
                opt => opt.Ignore());
    }
}
