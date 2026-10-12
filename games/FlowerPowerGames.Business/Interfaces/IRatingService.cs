using FlowerPowerGames.Business.DTOs;

namespace FlowerPowerGames.Business.Interfaces;

public interface IRatingService
{
    Task<List<RatingDto>> GetAllRatingsAsync();
    Task<List<RatingDto>> GetRatingsByGameIdAsync(int id);
    Task<List<RatingDto>> GetRatingsByUserIdAsync(int userId);
    Task<RatingDto> GetRatingByIdAsync(int id);
    Task<RatingDto> CreateRatingAsync(CreateRatingDto createRatingDto, int userId);
    Task<RatingDto> UpdateRatingAsync(int id, RatingDto ratingDto);
    Task<bool> DeleteRatingAsync(int id);

}