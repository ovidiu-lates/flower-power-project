using Microsoft.EntityFrameworkCore;
using FlowerPowerGames.Data.Models;
using System.Reflection.Emit;

namespace FlowerPowerGames.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<Game> Games=> Set<Game>();
    public DbSet<Rating> Ratings => Set<Rating>();

    public DbSet<Favorite> Favorites => Set<Favorite>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Game>()
            .Property(game => game.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Game>()
            .Property(game => game.Rating)
            .HasPrecision(3, 2);


        modelBuilder.Entity<Rating>()
            .HasOne(rating => rating.Game)
            .WithMany()
            .HasForeignKey(rating => rating.GameId)
            .IsRequired();

        modelBuilder.Entity<Favorite>()
            .HasOne(favorite => favorite.Game)
            .WithMany()
            .HasForeignKey(favorite => favorite.GameId)
            .IsRequired();
    }
}