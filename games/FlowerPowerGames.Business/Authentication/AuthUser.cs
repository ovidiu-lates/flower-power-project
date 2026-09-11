namespace FlowerPowerGames.Business.Authentication
{
    public sealed class AuthUser
    {
        public int Id { get; init; }

        public string Email { get; init; } = string.Empty;

        public string Username { get; init; } = string.Empty;

        public string FullName { get; init; } = string.Empty;

        public string Role { get; init; } = "User";

        public string PasswordHash { get; set; } = string.Empty;

        public bool IsActive { get; init; } = true;
    }
}
