using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FlowerPowerGames.Business.Interfaces;

namespace FlowerPowerGames.Business.Services;

public class AiUsageService : IAiUsageService
{
    public Task<AiUsageDTO> CreateAiUsageAsync(AiUsageDTO aiUsageDto)
    {
        throw new NotImplementedException();
    }

    public Task<AiUsageDTO> GetAiUsageByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<AiUsageDTO> GetAiUsageByUserIdAsync(int userId)
    {
        throw new NotImplementedException();
    }

    public Task<AiUsageDTO> UpdateAiUsageAsync(AiUsageDTO aiUsageDto)
    {
        throw new NotImplementedException();
    }
}
