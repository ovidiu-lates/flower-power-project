using System;
using System.Collections.Generic;
using System.Text;
using FlowerPowerGames.Business.DTOs;

namespace FlowerPowerGames.Business.Interfaces;

public interface IAiUsageService
{
    Task<AiUsageDTO> GetAiUsageByUserIdAsync(int userId);
    Task<AiUsageDTO> GetAiUsageByIdAsync(int id);
    Task<AiUsageDTO> CreateAiUsageAsync(CreateAiUsageDTO createAiUsageDto, int userId);
    Task<AiUsageDTO> UpdateAiUsageAsync(int id, AiUsageDTO aiUsageDto);
    Task<AiUsageDTO> UpdateAiUsageAdminAsync(int id, UpdateAiUsageDTO updateAiUsageDto);

    Task EnsureAiUsageAvailableAsync(int userId, CancellationToken cancellationToken = default);
    Task RecordAiRequestAsync(int userId, int tokensUsed, CancellationToken cancellationToken = default);
}