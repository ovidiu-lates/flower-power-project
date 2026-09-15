using Microsoft.EntityFrameworkCore;
using FlowerPowerGames.Data.Models;
using System.Reflection.Emit;

namespace FlowerPowerGames.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<Game> Games=> Set<Game>();

    public DbSet<Favorite> Favorites => Set<Favorite>();

    public DbSet<Genre> Genres => Set<Genre>();

    public DbSet<GameType> GameTypes => Set<GameType>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Game>()
            .Property(game => game.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Game>()
            .Property(game => game.Rating)
            .HasPrecision(3, 2);

        //need to configure for the user when it is created just like for the game

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
            .UsingEntity(join => join.ToTable("GameGameTypes")); 
    }
}