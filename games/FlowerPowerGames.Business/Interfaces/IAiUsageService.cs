using FlowerPowerGames.Business.DTOs;

namespace FlowerPowerGames.Business.Interfaces;

public interface IAiUsageService
{
    Task<AiUsageDto> GetAiUsageByUserIdAsync(int userId);
    Task<AiUsageDto> GetAiUsageByIdAsync(int id);
    Task<AiUsageDto> CreateAiUsageAsync(CreateAiUsageDto createAiUsageDto, int userId);
    Task<AiUsageDto> UpdateAiUsageAsync(int id, AiUsageDto aiUsageDto);
    Task<AiUsageDto> UpdateAiUsageAdminAsync(int id, UpdateAiUsageDto updateAiUsageDto);
}