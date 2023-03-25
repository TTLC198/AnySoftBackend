using AutoMapper;
using RPM_PR_LIB;

namespace RPM_Project_Backend.Mappings;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<ProductDto, Product>().ReverseMap();
    }
}