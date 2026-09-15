using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Data.Models;

namespace FlowerPowerGames.Business.Mappers;

public class AiUsageProfile : Profile
{
    public AiUsageProfile()
    {
        CreateMap<AiUsage, AiUsageDTO>();
        CreateMap<AiUsageDTO, AiUsage>()
            .ForMember(
                dest => dest.Id,
                opt => opt.Ignore());
    }

}
