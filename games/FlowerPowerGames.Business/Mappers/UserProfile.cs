using AutoMapper;
using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Data.Models;

namespace FlowerPowerGames.Business.Mappers;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>();

        CreateMap<UserDto, User>()
            .ForMember(
                destination => destination.Id,
                option => option.Ignore());
    }
}