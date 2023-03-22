using AutoMapper;
using RPM_PR_LIB;

namespace RPM_Project_Backend.Mappings;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<UserDto, User>().ReverseMap();
    }
}