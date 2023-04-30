using AutoMapper;
using RPM_Project_Backend.Domain;

namespace RPM_Project_Backend.Mappings;

public class GenreMappingProfile : Profile
{
    public GenreMappingProfile()
    {
        CreateMap<GenreDto, Genre>().ReverseMap();
    }
}