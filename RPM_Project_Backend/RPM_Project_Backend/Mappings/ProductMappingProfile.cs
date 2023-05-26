using AnySoftBackend.Library.DataTransferObjects.Product;
using AutoMapper;
using RPM_Project_Backend.Domain;

namespace RPM_Project_Backend.Mappings;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<ProductCreateDto, Product>().ReverseMap();
    }
}