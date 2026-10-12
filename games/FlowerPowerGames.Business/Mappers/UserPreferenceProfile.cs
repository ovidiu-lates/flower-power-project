using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Data.Models;

namespace FlowerPowerGames.Business.Mappers;
    
[ExcludeFromCodeCoverage]

public class UserPreferenceProfile : Profile
{
    public UserPreferenceProfile()
    {
        CreateMap<UserPreference, UserPreferenceDto>()
            .ForMember(
                dest => dest.GenreIds,
                opt => opt.MapFrom(
                    src => src.Genres.Select(g => g.Id)))
            .ForMember(
                dest => dest.TypeIds,
                opt => opt.MapFrom(
                    src => src.Types.Select(t => t.Id)));

        CreateMap<UserPreferenceDto, UserPreference>()
            .ForMember(
                dest => dest.Id,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.Genres,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.Types,
                opt => opt.Ignore());

        CreateMap<CreateUserPreferenceDto, UserPreference>()
            .ForMember(
                dest => dest.Id,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.UserId,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.User,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.Genres,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.Types,
                opt => opt.Ignore());
    }
        
}
