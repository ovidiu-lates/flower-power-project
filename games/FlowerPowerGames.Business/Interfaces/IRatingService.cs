using System;
using System.Collections.Generic;
using System.Text;
using FlowerPowerGames.Business.DTOs;

namespace FlowerPowerGames.Business.Interfaces;

public interface IRatingService
{
    Task<List<RatingDto>> GetAllRatingsAsync();
    Task<List<RatingDto>> GetRatingsByGameIdAsync(int id);
    Task<RatingDto?> GetRatingByIdAsync(int id);
    Task<RatingDto> CreateRatingAsync(RatingDto ratingDto);
    Task<RatingDto?> UpdateRatingAsync(int id, RatingDto ratingDto);
    Task<bool> DeleteRatingAsync(int id);

}