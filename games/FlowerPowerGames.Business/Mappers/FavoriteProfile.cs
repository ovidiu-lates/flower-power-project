using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Data.Models;

namespace FlowerPowerGames.Business.Mappers;

public class FavoriteProfile : Profile
{
    public FavoriteProfile()
    {
        CreateMap<Favorite, FavoriteDTO>();
        CreateMap<FavoriteDTO, Favorite>()
            .ForMember(
                dest => dest.Id,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.Game,
                opt => opt.Ignore());
    }
}
