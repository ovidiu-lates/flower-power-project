using FlowerPowerGames.Business.DTOs;

namespace FlowerPowerGames.Business.Interfaces;

public interface IGameService
{
    Task<List<GameDto>> GetAllGamesAsync();

    Task<GameDto> GetGameByIdAsync(int id);

    Task<GameDto> CreateGameAsync(GameDto gameDto);

    Task<GameDto> UpdateGameAsync(int id, GameDto gameDto);

    Task DeleteGameAsync(int id);

}
