using AutoMapper;
using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Data.Models;
using System;
using System.Collections.Generic;
using System.Runtime;
using System.Text;

namespace FlowerPowerGames.Business.Mappers;

public class GameProfile : Profile
{
    public GameProfile()
    {
        CreateMap<Game, GameDto>();

        CreateMap<GameDto, Game>()
            .ForMember(
                dest => dest.Id,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.Rating,
                opt => opt.Ignore());
    }
}