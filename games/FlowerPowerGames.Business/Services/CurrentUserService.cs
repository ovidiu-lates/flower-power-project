using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using FlowerPowerGames.Business.Interfaces;

namespace FlowerPowerGames.Business.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    public int? UserId
    {
        get
        {
            if (!IsAuthenticated) return null;

            var idClaim = User?.FindFirst(ClaimTypes.NameIdentifier) ??
                          User?.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);

            if (idClaim == null) return null;

            if (int.TryParse(idClaim.Value, out var id)) return id;
            return null;
        }
    }

    public string? Username => User?.FindFirst(ClaimTypes.Name)?.Value;

    public string? Role => User?.FindFirst(ClaimTypes.Role)?.Value;
}