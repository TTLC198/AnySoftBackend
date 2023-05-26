using AnySoftBackend.Library.DataTransferObjects.Property;
using AutoMapper;
using RPM_Project_Backend.Domain;

namespace RPM_Project_Backend.Mappings;

public class PropertyMappingProfile : Profile
{
    public PropertyMappingProfile()
    {
        CreateMap<PropertyCreateDto, Property>().ReverseMap();
        CreateMap<PropertyDto, Property>().ReverseMap();
        CreateMap<PropertyCreateDto, PropertyDto>().ReverseMap();
    }
}