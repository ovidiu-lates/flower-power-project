using AutoMapper;
using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Data.Models;
using System;
using System.Collections.Generic;
using System.Runtime;
using System.Text;

namespace FlowerPowerGames.Business.Mappers;

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
    }
}