using AutoMapper;
using RPM_Project_Backend.Domain;

namespace RPM_Project_Backend.Mappings;

public class ReviewMappingProfile : Profile
{
    public ReviewMappingProfile()
    {
        CreateMap<ReviewDto, Review>().ReverseMap();
    }
}