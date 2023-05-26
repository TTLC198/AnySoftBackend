using AnySoftBackend.Library.DataTransferObjects.User;
using AutoMapper;
using RPM_Project_Backend.Domain;

namespace RPM_Project_Backend.Mappings;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<UserCreateDto, User>().ReverseMap();
    }
}