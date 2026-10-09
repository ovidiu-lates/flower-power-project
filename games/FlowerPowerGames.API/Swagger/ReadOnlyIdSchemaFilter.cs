using FlowerPowerGames.Business.DTOs;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FlowerPowerGames.API.Swagger;

public class ReadOnlyIdSchemaFilter : ISchemaFilter
{
    private static readonly Dictionary<Type, string> ReadOnlyIdProperties = new()
    {
        { typeof(GameDto), "id" },
        { typeof(FavoriteDto), "id" },
        { typeof(GameTypeDto), "id" },
        { typeof(GenreDto), "id" },
        { typeof(RatingDto), "id" },
        { typeof(UserDto), "id" },
        { typeof(RoleDto), "roleId" },
        { typeof(AiUsageDto), "id" },
        { typeof(UserPreferenceDto), "id" }
    };

    public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
    {
        if (!ReadOnlyIdProperties.TryGetValue(
                context.Type,
                out var propertyName))
        {
            return;
        }

        if (schema is not OpenApiSchema concreteSchema ||
            concreteSchema.Properties is null)
        {
            return;
        }

        if (concreteSchema.Properties.TryGetValue(
                propertyName,
                out var property) &&
            property is OpenApiSchema concreteProperty)
        {
            concreteProperty.ReadOnly = true;
        }
    }
}