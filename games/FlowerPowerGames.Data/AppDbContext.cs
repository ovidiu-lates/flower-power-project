using Microsoft.EntityFrameworkCore;
using FlowerPowerGames.Data.Models;
using System.Reflection.Emit;

namespace FlowerPowerGames.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<Game> Games=> Set<Game>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Game>()
            .Property(game => game.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Game>()
            .Property(game => game.Rating)
            .HasPrecision(3, 2);
    }
}