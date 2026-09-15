using System;
using System.Collections.Generic;
using System.Text;
using FlowerPowerGames.Business.DTOs;

namespace FlowerPowerGames.Business.Interfaces;

public interface IAiUsageService
{
    Task<AiUsageDTO> GetAiUsageByUserIdAsync(int userId);
    Task<AiUsageDTO> GetAiUsageByIdAsync(int id);
    Task<AiUsageDTO> CreateAiUsageAsync(AiUsageDTO aiUsageDto);
    Task<AiUsageDTO> UpdateAiUsageAsync(AiUsageDTO aiUsageDto);
}
