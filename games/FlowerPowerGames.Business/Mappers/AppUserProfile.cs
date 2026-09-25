using AutoMapper;
using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Data.Models;

namespace FlowerPowerGames.Business.Mappers;

public class AppUserProfile : Profile
{
    public AppUserProfile()
    {
        CreateMap<AppUser, AppUserDto>();

        CreateMap<AppUserDto, AppUser>()
            .ForMember(
                destination => destination.UserId,
                option => option.Ignore());
    }
}