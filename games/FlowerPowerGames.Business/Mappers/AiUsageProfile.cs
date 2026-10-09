using System.Diagnostics.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Data.Models;

namespace FlowerPowerGames.Business.Mappers;
    [ExcludeFromCodeCoverage]

public class AiUsageProfile : Profile
{
    public AiUsageProfile()
    {
        CreateMap<AiUsage, AiUsageDTO>();
        CreateMap<AiUsageDTO, AiUsage>()
            .ForMember(
                dest => dest.Id,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.TotalAvailablePrompt,
                opt => opt.Ignore());

        CreateMap<CreateAiUsageDTO, AiUsage>()
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
                dest => dest.TotalPromptUsed,
                opt => opt.Ignore())
            .ForMember(
                dest => dest.TotalRequests,
                opt => opt.Ignore());

        CreateMap<UpdateAiUsageDTO, AiUsage>();
    }

}
