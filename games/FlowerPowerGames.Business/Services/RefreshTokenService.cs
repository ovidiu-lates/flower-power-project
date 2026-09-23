using FlowerPowerGames.Business.Authentication;
using FlowerPowerGames.Business.Interfaces;
using FlowerPowerGames.Data;
using FlowerPowerGames.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FlowerPowerGames.Business.Services;

public sealed class RefreshTokenService
    : IRefreshTokenService
{
    private readonly AppDbContext _context;

    public RefreshTokenService(
        AppDbContext context)
    {
        _context = context;
    }

    public async Task SaveAsync(
        RefreshToken record)
    {
        var session = new AuthSession
        {
            UserId = record.UserId,
            RefreshTokenHash = record.TokenHash,
            ExpiresAtUtc = record.ExpiresAtUtc,
            CreatedAtUtc = DateTime.UtcNow
        };

        _context.AuthSessions.Add(session);

        await _context.SaveChangesAsync();
    }

    public async Task<RefreshToken?> FindAsync(
        string tokenHash)
    {
        var session = await _context.AuthSessions
            .AsNoTracking()
            .FirstOrDefaultAsync(session =>
                session.RefreshTokenHash == tokenHash);

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

    public async Task RevokeAsync(
        string tokenHash)
    {
        var session = await _context.AuthSessions
            .FirstOrDefaultAsync(session =>
                session.RefreshTokenHash == tokenHash);

        if (session is null)
        {
            return;
        }

        session.RevokedAtUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }
}