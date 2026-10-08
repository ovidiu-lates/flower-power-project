using System;
using System.Collections.Generic;
using System.Text;
using FlowerPowerGames.Business.DTOs;

namespace FlowerPowerGames.Business.Interfaces;

public interface IRatingService
{
    Task<List<RatingDto>> GetAllRatingsAsync();
    Task<List<RatingDto>> GetRatingsByGameIdAsync(int id);
    Task<List<RatingDto>> GetRatingsByUserIdAsync(int id);
    Task<RatingDto> GetRatingByIdAsync(int id);
    Task<RatingDto> CreateRatingAsync(CreateRatingDTO ratingDTO, int userId);
    Task<RatingDto> UpdateRatingAsync(int id, RatingDto ratingDTO);
    Task<bool> DeleteRatingAsync(int id);

}