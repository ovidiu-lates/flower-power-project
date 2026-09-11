using FlowerPowerGames.Business.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlowerPowerGames.Business.Interfaces;

public interface IGameTypeService
{
    Task<List<GameTypeDto>> GetAllGameTypesAsync();

    Task<GameTypeDto?> GetGameTypeByIdAsync(int id);

    Task<GameTypeDto> CreateGameTypeAsync(GameTypeDto gameTypeDto);

    Task<GameTypeDto?> UpdateGameTypeAsync(int id, GameTypeDto gameTypeDto);
}
