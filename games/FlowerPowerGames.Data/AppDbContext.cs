using FlowerPowerGames.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FlowerPowerGames.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<Game> Games => Set<Game>();

    public DbSet<Favorite> Favorites => Set<Favorite>();

    public DbSet<Genre> Genres => Set<Genre>();

    public DbSet<GameType> GameTypes => Set<GameType>();

    public DbSet<Role> Roles => Set<Role>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Game configuration
        modelBuilder.Entity<Game>()
            .Property(game => game.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Game>()
            .Property(game => game.Rating)
            .HasPrecision(3, 2);

        modelBuilder.Entity<Game>()
            .HasIndex(game => game.Name)
            .IsUnique();

        // Favorite configuration
        modelBuilder.Entity<Favorite>()
            .HasOne(favorite => favorite.Game)
            .WithMany()
            .HasForeignKey(favorite => favorite.GameId)
            .IsRequired();

        // Genre configuration
        modelBuilder.Entity<Genre>()
            .HasIndex(genre => genre.Name)
            .IsUnique();

        // GameType configuration
        modelBuilder.Entity<GameType>()
            .HasIndex(type => type.Name)
            .IsUnique();

        // Game - Genre many-to-many relationship
        modelBuilder.Entity<Game>()
            .HasMany(game => game.Genres)
            .WithMany(genre => genre.Games)
            .UsingEntity(join =>
                join.ToTable("GameGenres"));

        // Game - GameType many-to-many relationship
        modelBuilder.Entity<Game>()
            .HasMany(game => game.Types)
            .WithMany(type => type.Games)
            .UsingEntity(join =>
                join.ToTable("GameGameTypes"));

        // Role configuration
        modelBuilder.Entity<Role>()
            .HasKey(role => role.RoleId);

        modelBuilder.Entity<Role>()
            .Property(role => role.Name)
            .IsRequired()
            .HasMaxLength(100);

        modelBuilder.Entity<Role>()
            .HasIndex(role => role.Name)
            .IsUnique();
    }
}