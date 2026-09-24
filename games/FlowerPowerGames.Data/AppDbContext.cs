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

    public DbSet<AppUser> AppUsers => Set<AppUser>();

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

        // AppUser configuration
        modelBuilder.Entity<AppUser>()
            .HasKey(user => user.UserId);

        modelBuilder.Entity<AppUser>()
            .Property(user => user.Email)
            .IsRequired()
            .HasMaxLength(255);

        modelBuilder.Entity<AppUser>()
            .Property(user => user.PasswordHash)
            .IsRequired()
            .HasMaxLength(255);

        modelBuilder.Entity<AppUser>()
            .Property(user => user.FullName)
            .IsRequired()
            .HasMaxLength(150);

        modelBuilder.Entity<AppUser>()
            .Property(user => user.Username)
            .IsRequired()
            .HasMaxLength(100);

        modelBuilder.Entity<AppUser>()
            .HasIndex(user => user.Email)
            .IsUnique();

        modelBuilder.Entity<AppUser>()
            .HasIndex(user => user.Username)
            .IsUnique();

        modelBuilder.Entity<AppUser>()
            .HasOne(user => user.Role)
            .WithMany()
            .HasForeignKey(user => user.RoleId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AppUser>()
            .ToTable("APP_USER");
    }
}