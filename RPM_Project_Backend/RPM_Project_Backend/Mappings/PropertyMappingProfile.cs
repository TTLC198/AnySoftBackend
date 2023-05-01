using AutoMapper;
using RPM_Project_Backend.Domain;

namespace RPM_Project_Backend.Mappings;

public class PropertyMappingProfile : Profile
{
    public PropertyMappingProfile()
    {
        CreateMap<PropertyDto, Property>().ReverseMap();
    }
}