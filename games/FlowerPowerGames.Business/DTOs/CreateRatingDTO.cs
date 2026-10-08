using System;
using System.Collections.Generic;
using System.Text;

namespace FlowerPowerGames.Business.DTOs;

public class CreateRatingDTO
{
    public int GameId { get; set; }

    public int Score { get; set; }

    public string? Review { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
