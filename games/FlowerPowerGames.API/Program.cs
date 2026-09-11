using FlowerPowerGames.Business.Interfaces;
using FlowerPowerGames.Business.Mappers;
using FlowerPowerGames.Business.Services;
using FlowerPowerGames.Data;
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

// Add services to the container.

builder.Services.AddScoped<IGameService, GameService>();

builder.Services.AddScoped<IFavoriteService, FavoriteService>();

builder.Services.AddAutoMapper(
    cfg => { },
    typeof(GameProfile).Assembly);

builder.Services.AddAutoMapper(
    cfg => { },
    typeof(FavoriteProfile).Assembly);

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
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
