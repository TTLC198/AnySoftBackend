using AnySoftBackend.Library.DataTransferObjects.Product;
using AutoMapper;
using AnySoftBackend.Domain;

namespace AnySoftBackend.Mappings;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<ProductCreateDto, Product>().ReverseMap();
    }
}