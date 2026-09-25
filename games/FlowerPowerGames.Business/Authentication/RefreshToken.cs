namespace FlowerPowerGames.Business.Authentication;

public sealed class RefreshToken
{
    public int UserId { get; init; }

    public string TokenHash { get; init; } = string.Empty;

    public DateTime ExpiresAtUtc { get; init; }

    public DateTime? RevokedAtUtc { get; set; }
}