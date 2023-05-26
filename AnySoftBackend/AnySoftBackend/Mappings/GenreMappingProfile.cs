using AnySoftBackend.Library.DataTransferObjects;
using AutoMapper;
using AnySoftBackend.Domain;
using AnySoftBackend.Library.DataTransferObjects.Genre;

namespace AnySoftBackend.Mappings;

public class GenreMappingProfile : Profile
{
    public GenreMappingProfile()
    {
        CreateMap<GenreDto, Genre>().ReverseMap();
        CreateMap<GenreCreateDto, Genre>().ReverseMap();
        CreateMap<GenreCreateDto, GenreDto>().ReverseMap();
    }
}