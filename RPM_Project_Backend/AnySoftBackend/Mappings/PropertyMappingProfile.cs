using AnySoftBackend.Library.DataTransferObjects.Property;
using AutoMapper;
using AnySoftBackend.Domain;

namespace AnySoftBackend.Mappings;

public class PropertyMappingProfile : Profile
{
    public PropertyMappingProfile()
    {
        CreateMap<PropertyCreateDto, Property>().ReverseMap();
        CreateMap<PropertyDto, Property>().ReverseMap();
        CreateMap<PropertyCreateDto, PropertyDto>().ReverseMap();
    }
}