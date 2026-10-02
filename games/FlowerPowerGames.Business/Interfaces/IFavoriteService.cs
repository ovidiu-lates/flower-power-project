using System;
using System.Collections.Generic;
using System.Text;
using FlowerPowerGames.Business.DTOs;

namespace FlowerPowerGames.Business.Interfaces;

public interface IFavoriteService
{
    Task<List<FavoriteDTO>> GetAllFavoritesAsync();
    Task<List<FavoriteDTO>> GetFavoritesByUserIdAsync(int Id);
    Task<FavoriteDTO> GetFavoriteByIdAsync(int id);
    Task<FavoriteDTO> CreateFavoriteAsync(CreateFavoriteDTO favoriteDto, int userId);
    Task<bool> DeleteFavoriteAsync(int id, int userId, string role);
}
