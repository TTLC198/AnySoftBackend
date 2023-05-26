using AnySoftBackend.Library.DataTransferObjects.User;
using AutoMapper;
using AnySoftBackend.Domain;

namespace AnySoftBackend.Mappings;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<UserCreateDto, User>().ReverseMap();
    }
}