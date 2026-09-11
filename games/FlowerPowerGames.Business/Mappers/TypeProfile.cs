using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Data.Models;

namespace FlowerPowerGames.Business.Mappers;

public class TypeProfile : Profile
{
    public TypeProfile()
    {
        CreateMap<GameType, GameTypeDto>();
        CreateMap<GameTypeDto, GameType>()
            .ForMember(
                dest => dest.Id,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.Games,
                opt => opt.Ignore());
    }
}
