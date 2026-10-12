using FlowerPowerGames.Business.DTOs;

namespace FlowerPowerGames.Business.Interfaces;

public interface IGameTypeService
{
    Task<List<GameTypeDto>> GetAllGameTypesAsync();

    Task<GameTypeDto> GetGameTypeByIdAsync(int id);

    Task<GameTypeDto> CreateGameTypeAsync(GameTypeDto gameTypeDto);

    Task<GameTypeDto> UpdateGameTypeAsync(int id, GameTypeDto gameTypeDto);

    Task<bool> DeleteGameTypeAsync(int id);
}
