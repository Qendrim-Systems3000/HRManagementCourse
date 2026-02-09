using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HRManagement.Api;

internal static class SwaggerJwtExtensions
{
    public static void AddJwtSecurity(this SwaggerGenOptions options)
    {
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "Paste your JWT from the login response. Enter only the token (Swagger will add 'Bearer ' for you).",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT"
        });
        options.AddSecurityRequirement(document =>
        {
            var schemeRef = new OpenApiSecuritySchemeReference("Bearer", document);
            var requirement = new OpenApiSecurityRequirement
            {
                { schemeRef, [] }
            };
            return requirement;
        });
    }
}
