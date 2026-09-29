using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FlowerPowerGames.Business.DTOs;

public class FavoriteDTO
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int GameId { get; set; }
}
