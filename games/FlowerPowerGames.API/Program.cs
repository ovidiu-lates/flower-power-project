using FlowerPowerGames.Business.Interfaces;
using FlowerPowerGames.Business.Mappers;
using FlowerPowerGames.Business.Services;
using FlowerPowerGames.Data;
using FlowerPowerGames.Data.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' was not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        connectionString,
        sqlOptions => sqlOptions.EnableRetryOnFailure()));

// Game services
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<IGameTypeService, GameTypeService>();

// Favorite service
builder.Services.AddScoped<IFavoriteService, FavoriteService>();

// Role services
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRoleService, RoleService>();

// AutoMapper
builder.Services.AddAutoMapper(
    cfg => { },
    typeof(GameProfile).Assembly);

builder.Services.AddAutoMapper(
    cfg => { },
    typeof(FavoriteProfile).Assembly);

// Controllers and API documentation
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();