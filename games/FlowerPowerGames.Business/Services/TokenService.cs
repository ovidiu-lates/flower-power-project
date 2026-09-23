using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FlowerPowerGames.Business.Authentication;
using FlowerPowerGames.Business.Interfaces;
using FlowerPowerGames.Data;
using FlowerPowerGames.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FlowerPowerGames.Business.Services;

public sealed class TokenService : ITokenService
{
    private readonly JwtSettings _settings;
    private readonly AppDbContext _context;

    public TokenService(IOptions<JwtSettings> options, AppDbContext context)
    {
        _settings = options.Value;
        _context = context;
    }

    public TokenPair CreateTokenPair(AuthUser user)
    {
        var now = DateTime.UtcNow;

        var accessTokenExpiresAt = now.AddMinutes(_settings.AccessTokenMinutes);

        var refreshTokenExpiresAt = now.AddDays(_settings.RefreshTokenDays);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),

            new(ClaimTypes.NameIdentifier, user.Id.ToString()),

            new(ClaimTypes.Name, user.Username),

            new(ClaimTypes.Email, user.Email),

            new(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes( _settings.SecretKey));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            notBefore: now,
            expires: accessTokenExpiresAt,
            signingCredentials: credentials);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);

        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        return new TokenPair
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiresAtUtc = accessTokenExpiresAt,
            RefreshTokenExpiresAtUtc = refreshTokenExpiresAt
        };
    }

    public string HashRefreshToken(string refreshToken)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken));

        return Convert.ToBase64String(bytes);
    }

    public async Task SaveAsync(RefreshToken refreshToken)
    {
        var session = new AuthSession
        {
            UserId = refreshToken.UserId,
            RefreshTokenHash = refreshToken.TokenHash,
            ExpiresAtUtc = refreshToken.ExpiresAtUtc,
            CreatedAtUtc = DateTime.UtcNow
        };

        _context.AuthSessions.Add(session);

        await _context.SaveChangesAsync();
    }

    public async Task<RefreshToken?> FindAsync(string tokenHash)
    {
        var session = await _context.AuthSessions
            .AsNoTracking()
            .FirstOrDefaultAsync(session => session.RefreshTokenHash == tokenHash);

        if (session is null)
        {
            return null;
        }

        return new RefreshToken
        {
            UserId = session.UserId,
            TokenHash = session.RefreshTokenHash,
            ExpiresAtUtc = session.ExpiresAtUtc,
            RevokedAtUtc = session.RevokedAtUtc
        };
    }

    public async Task RevokeAsync(string tokenHash)
    {
        var session = await _context.AuthSessions
            .FirstOrDefaultAsync(session => session.RefreshTokenHash == tokenHash);

        if (session is null)
        {
            return;
        }

        session.RevokedAtUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }
}