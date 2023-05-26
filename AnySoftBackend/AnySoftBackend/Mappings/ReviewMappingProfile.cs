using AnySoftBackend.Library.DataTransferObjects.Review;
using AutoMapper;
using AnySoftBackend.Domain;

namespace AnySoftBackend.Mappings;

public class ReviewMappingProfile : Profile
{
    public ReviewMappingProfile()
    {
        CreateMap<ReviewCreateDto, Review>().ReverseMap();
    }
}