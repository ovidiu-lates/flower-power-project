using FlowerPowerGames.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FlowerPowerGames.Data;

public class AppDbContext(
    DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<Game> Games => Set<Game>();

    public DbSet<Favorite> Favorites => Set<Favorite>();

    public DbSet<Genre> Genres => Set<Genre>();

    public DbSet<GameType> GameTypes => Set<GameType>();

    public DbSet<AuthSession> AuthSessions => Set<AuthSession>();

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Game>()
            .Property(game => game.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Game>()
            .Property(game => game.Rating)
            .HasPrecision(3, 2);

        modelBuilder.Entity<Favorite>()
            .HasOne(favorite => favorite.Game)
            .WithMany()
            .HasForeignKey(favorite => favorite.GameId)
            .IsRequired();

        modelBuilder.Entity<Game>()
            .HasIndex(game => game.Name)
            .IsUnique();

        modelBuilder.Entity<Genre>()
            .HasIndex(genre => genre.Name)
            .IsUnique();

        modelBuilder.Entity<GameType>()
            .HasIndex(type => type.Name)
            .IsUnique();

        modelBuilder.Entity<Game>()
            .HasMany(game => game.Genres)
            .WithMany(genre => genre.Games)
            .UsingEntity(join =>
                join.ToTable("GameGenres"));

        modelBuilder.Entity<Game>()
            .HasMany(game => game.Types)
            .WithMany(type => type.Games)
            .UsingEntity(join =>
                join.ToTable("GameGameTypes"));

        modelBuilder.Entity<AuthSession>()
            .HasIndex(session =>
                session.RefreshTokenHash)
            .IsUnique();

        modelBuilder.Entity<AuthSession>()
            .HasOne(session => session.User)
            .WithMany(user => user.AuthSessions)
            .HasForeignKey(session => session.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");

            entity.HasKey(user => user.Id);

            entity.Property(user => user.Email)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(user => user.Username)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(user => user.FullName)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(user => user.PasswordHash)
                .IsRequired();

            entity.HasIndex(user => user.Email)
                .IsUnique();

            entity.HasIndex(user => user.Username)
                .IsUnique();
        });
    }
}