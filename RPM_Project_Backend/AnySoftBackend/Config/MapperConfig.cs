using AnySoftBackend.Mappings;

namespace AnySoftBackend.Config;

public static class MapperConfig
{
    public static void AddAutoMapperConfiguration(this IServiceCollection services)
    {
        services
            .AddAutoMapper(typeof(UserMappingProfile));
        services
            .AddAutoMapper(typeof(ProductMappingProfile));
        services
            .AddAutoMapper(typeof(ReviewMappingProfile));
        services
            .AddAutoMapper(typeof(GenreMappingProfile));
        services
            .AddAutoMapper(typeof(PropertyMappingProfile));
        services
            .AddAutoMapper(typeof(PaymentMappingProfile));
        services
            .AddAutoMapper(typeof(OrderMappingProfile));
    }
}