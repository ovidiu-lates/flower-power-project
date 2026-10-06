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
    Task<AiUsageDTO> UpdateAiUsageAsync(int id, AiUsageDTO aiUsageDto);

    Task EnsureAiUsageAvailableAsync(int userId, CancellationToken cancellationToken = default);
    Task RecordAiRequestAsync(int userId, CancellationToken cancellationToken = default);
}