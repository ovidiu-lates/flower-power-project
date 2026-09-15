using System;
using System.Collections.Generic;
using System.Text;

namespace FlowerPowerGames.Data.Models;

public class AiUsage
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int TotalRequests { get; set; }
    public int TotalPromptUsed { get; set; } = 0;
    public int TotalAvailablePrompt { get; set; } = 750;
}
