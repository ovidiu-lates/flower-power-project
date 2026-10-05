using AutoMapper;
using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlowerPowerGames.Business.Mappers;

public class UserPreferenceProfile : Profile
{
    public UserPreferenceProfile()
    {
        CreateMap<UserPreference, UserPreferenceDTO>()
            .ForMember(
                dest => dest.GenreIds,
                opt => opt.MapFrom(
                    src => src.Genres.Select(g => g.Id)))
            .ForMember(
                dest => dest.TypeIds,
                opt => opt.MapFrom(
                    src => src.Types.Select(t => t.Id)));

        CreateMap<UserPreferenceDTO, UserPreference>()
            .ForMember(
                dest => dest.Id,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.Genres,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.Types,
                opt => opt.Ignore());
    }
        
}
