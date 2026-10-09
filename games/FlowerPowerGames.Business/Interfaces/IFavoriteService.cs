using System;
using System.Collections.Generic;
using System.Text;
using FlowerPowerGames.Business.DTOs;

namespace FlowerPowerGames.Business.Interfaces;

public interface IFavoriteService
{
    Task<List<FavoriteDto>> GetAllFavoritesAsync();
    Task<List<FavoriteDto>> GetFavoritesByUserIdAsync(int userId);
    Task<FavoriteDto> GetFavoriteByIdAsync(int id);
    Task<FavoriteDto> CreateFavoriteAsync(CreateFavoriteDto favoriteDto, int userId);
    Task<bool> DeleteFavoriteAsync(int id, int userId, string role);
}
