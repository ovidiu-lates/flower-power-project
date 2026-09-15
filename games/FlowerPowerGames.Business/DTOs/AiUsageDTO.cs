using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FlowerPowerGames.Business.DTOs;

public class AiUsageDTO
{
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    public int TotalRequests { get; set; }

    public int TotalPromptUsed { get; set; }

    public int TotalAvailablePrompt { get; set; }

}
