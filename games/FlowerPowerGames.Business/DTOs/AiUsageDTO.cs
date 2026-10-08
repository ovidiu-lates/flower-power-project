using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace FlowerPowerGames.Business.DTOs;

[ExcludeFromCodeCoverage]
public class AiUsageDTO
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int TotalRequests { get; set; }

    public int TotalPromptUsed { get; set; }

    public int TotalAvailablePrompt { get; set; }

}
