using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;


namespace FlowerPowerGames.Business.DTOs;

public class UserPreferenceDTO
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public decimal MinBudget { get; set; }
    public decimal MaxBudget { get; set; }

    public int MinPlayTime { get; set; }

    public int MaxPlayTime { get; set; }

    public int MinPlayers { get; set; }

    public int MaxPlayers { get; set; }


    public int MinimumAge { get; set; }

    public ICollection<int> GenreIds { get; set; } = [];

    public ICollection<int> TypeIds { get; set; } = [];
}
