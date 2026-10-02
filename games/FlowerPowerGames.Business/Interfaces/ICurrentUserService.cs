namespace FlowerPowerGames.Business.Interfaces;

public interface ICurrentUserService
{
    bool IsAuthenticated { get; }
    int? UserId { get; } 
    string? Username { get; }
    string? Role { get; }
}