using System;
using System.Collections.Generic;
using System.Text;
using FlowerPowerGames.Business.DTOs;

namespace FlowerPowerGames.Business.Interfaces;

public interface IFavoriteService
{
    Task<List<FavoriteDTO>> GetAllFavoritesAsync();
    Task<List<FavoriteDTO>> GetFavoritesByUserIdAsync(int Id);
    Task<FavoriteDTO?> GetFavoriteByIdAsync(int id);
    Task<FavoriteDTO> CreateFavoriteAsync(FavoriteDTO  favoriteDto);
    Task<FavoriteDTO?> UpdateFavoriteAsync(int id, FavoriteDTO favoriteDto);
    Task<bool> DeleteFavoriteAsync(int id);
}
