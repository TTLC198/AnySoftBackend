using RPM_Project_Backend.Mappings;

namespace RPM_Project_Backend.Config;

public static class MapperConfig
{
    public static void AddAutoMapperConfiguration(this IServiceCollection services)
    {
        services
            .AddAutoMapper(typeof(UserMappingProfile));
        services
            .AddAutoMapper(typeof(ProductMappingProfile));
    }
}