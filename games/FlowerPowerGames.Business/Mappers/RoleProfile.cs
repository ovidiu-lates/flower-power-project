using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Data.Models;
using System.Data;

namespace FlowerPowerGames.Business.Mappers;
    [ExcludeFromCodeCoverage]

public class RoleProfile : Profile
{
    public RoleProfile()
    {
        CreateMap<Role, RoleDto>();

        CreateMap<RoleDto, Role>()
            .ForMember(
                destination => destination.RoleId,
                option => option.Ignore());
    }
}
