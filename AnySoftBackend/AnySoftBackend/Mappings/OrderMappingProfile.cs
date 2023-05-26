using AnySoftBackend.Library.DataTransferObjects.Order;
using AutoMapper;
using AnySoftBackend.Domain;

namespace AnySoftBackend.Mappings;

public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        CreateMap<OrderResponseDto, Order>().ReverseMap();
    }
}