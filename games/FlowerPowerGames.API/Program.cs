using FlowerPowerGames.API.ExceptionHandling;
using FlowerPowerGames.Business.Authentication;
using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Interfaces;
using FlowerPowerGames.Business.Mappers;
using FlowerPowerGames.Business.Services;
using FlowerPowerGames.Business.Validators;
using FlowerPowerGames.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

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
builder.Services.AddScoped<IRatingService, RatingService>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<IGameTypeService, GameTypeService>();

builder.Services.AddScoped<IFavoriteService, FavoriteService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserPreferenceService, UserPreferenceService>();
builder.Services.AddScoped<IAiUsageService, AiUsageService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Jwt"));

var jwtSettings = builder.Configuration
    .GetSection("Jwt")
    .Get<JwtSettings>()
    ?? throw new InvalidOperationException(
        "JWT settings are missing.");

var signingKey = new SymmetricSecurityKey(
    Encoding.UTF8.GetBytes(
        jwtSettings.SecretKey));

builder.Services.AddAuthentication(
    JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,

            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,

            ValidateLifetime = true,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,

            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddSingleton<IPasswordHasher<AuthUser>,PasswordHasher<AuthUser>>();

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<ITokenService,TokenService>();

builder.Services.AddScoped<IAuthService,AuthService>();

// Validators
builder.Services.AddScoped<IValidator<AiUsageDTO>, AiUsageDTOValidator>();
builder.Services.AddScoped<IValidator<UserPreferenceDTO>, UserPreferenceDTOValidator>();
builder.Services.AddScoped<IValidator<RatingDto>, RatingDtoValidator>();
builder.Services.AddScoped<IValidator<GameDto>, GameDtoValidator>();
builder.Services.AddScoped<IValidator<UserDto>, UserDtoValidator>();
builder.Services.AddScoped<IValidator<RegisterRequestDTO>, RegisterRequestDTOValidator>();
builder.Services.AddScoped<IValidator<LoginRequestDTO>, LoginRequestDTOValidator>();
builder.Services.AddScoped<IValidator<FavoriteDTO>, FavoriteDTOValidator>();
builder.Services.AddScoped<IValidator<RefreshTokenRequestDTO>, RefreshTokenRequestDTOValidator>();
builder.Services.AddScoped<IValidator<GenreDto>, GenreDtoValidator>();
builder.Services.AddScoped<IValidator<GameTypeDto>, GameTypeDtoValidator>();
builder.Services.AddScoped<IValidator<RoleDto>, RoleDtoValidator>();
builder.Services.AddScoped<IValidator<CreateFavoriteDTO>, CreateFavoriteDTOValidator>();

builder.Services.AddAutoMapper(
    cfg => { },
    typeof(GameProfile).Assembly);

builder.Services.AddAutoMapper(
    cfg => { },
    typeof(FavoriteProfile).Assembly);


builder.Services.AddAutoMapper(
    cfg => { },
    typeof(RatingProfile).Assembly);

builder.Services.AddAutoMapper(
    cfg => { },
    typeof(AiUsageProfile).Assembly);

builder.Services.AddAutoMapper(
    cfg => { },
    typeof(UserPreferenceProfile).Assembly);


builder.Services.AddControllers();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description =
                "Enter your JWT access token."
        });

    options.AddSecurityRequirement(
        document => new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(
                "Bearer",
                document)] = []
        });
});
var app = builder.Build();
app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthentication();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
