using AutoMapper;
using RPM_Project_Backend.Domain;

namespace RPM_Project_Backend.Mappings;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<ProductDto, Product>().ReverseMap();
    }
}