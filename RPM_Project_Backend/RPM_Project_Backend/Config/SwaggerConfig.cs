using Microsoft.OpenApi.Models;

namespace RPM_Project_Backend.Config;

public static class SwaggerConfig
{
    public static void AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "RPM Project Rest API",
                Description = "Developed by Vadim Kh. & Andrey K. 2023",
                Contact = new OpenApiContact { Name = "Vadim Kh.", Email = "yorehacked@gmail.com" }
            });

            c.IncludeXmlComments(Path.Combine($"{System.AppDomain.CurrentDomain.BaseDirectory}", "RPM_Project_Backend.xml"));
            c.IncludeXmlComments(Path.Combine($"{System.AppDomain.CurrentDomain.BaseDirectory}", "RPM_Project_Backend.Domain.xml"));

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Input the JWT like: Bearer {your token}",
                Name = "Authorization",
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });
        });
    }

    public static void UseSwaggerConfiguration(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        });
    }
}