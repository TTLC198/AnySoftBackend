using AnySoftBackend.Library.DataTransferObjects.Review;
using AutoMapper;
using RPM_Project_Backend.Domain;

namespace RPM_Project_Backend.Mappings;

public class ReviewMappingProfile : Profile
{
    public ReviewMappingProfile()
    {
        CreateMap<ReviewCreateDto, Review>().ReverseMap();
    }
}