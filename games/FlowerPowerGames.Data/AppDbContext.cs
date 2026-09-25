using FlowerPowerGames.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FlowerPowerGames.Data;

public class AppDbContext(
    DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<Game> Games => Set<Game>();

    public DbSet<Rating> Ratings => Set<Rating>();

    public DbSet<Favorite> Favorites => Set<Favorite>();

    public DbSet<Genre> Genres => Set<Genre>();

    public DbSet<GameType> GameTypes => Set<GameType>();

    public DbSet<AuthSession> AuthSessions => Set<AuthSession>();

    public DbSet<User> Users => Set<User>();

    public DbSet<Role> Roles => Set<Role>();

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

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Roles");

            entity.HasKey(role => role.RoleId);

            entity.Property(role => role.Name)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasIndex(role => role.Name)
                .IsUnique();

            entity.HasData(
                new Role
                {
                    RoleId = 1,
                    Name = "User"
                },
                new Role
                {
                    RoleId = 2,
                    Name = "Admin"
                });
        });

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

            entity.HasOne(user => user.Role)
                .WithMany(role => role.Users)
                .HasForeignKey(user => user.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<AuthSession>(entity =>
        {
            entity.ToTable("AuthSessions");

            entity.HasKey(session => session.Id);

            entity.Property(session =>
                session.RefreshTokenHash)
                .IsRequired();

            entity.HasIndex(session =>
                session.RefreshTokenHash)
                .IsUnique();

            entity.HasOne(session => session.User)
                .WithMany(user => user.AuthSessions)
                .HasForeignKey(session => session.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}