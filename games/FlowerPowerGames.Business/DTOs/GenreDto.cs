using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;


namespace FlowerPowerGames.Business.DTOs;

public class GenreDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
}
