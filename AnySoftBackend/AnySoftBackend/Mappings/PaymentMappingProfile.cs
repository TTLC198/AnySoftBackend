using AnySoftBackend.Library.DataTransferObjects.Payment;
using AutoMapper;
using AnySoftBackend.Domain;

namespace AnySoftBackend.Mappings;

public class PaymentMappingProfile : Profile
{
    public PaymentMappingProfile()
    {
        CreateMap<PaymentCreateDto, Payment>().ReverseMap();
    }
}