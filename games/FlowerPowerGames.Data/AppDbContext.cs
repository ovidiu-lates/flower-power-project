using Microsoft.EntityFrameworkCore;
using FlowerPowerGames.Data.Models;
using System.Reflection.Emit;

namespace FlowerPowerGames.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();

    public DbSet<Role> Roles => Set<Role>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>()
            .Property(product => product.Price)
            .HasPrecision(18, 2);

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