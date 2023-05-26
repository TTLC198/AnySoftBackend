using AnySoftBackend.Library.DataTransferObjects.Payment;
using AutoMapper;
using RPM_Project_Backend.Domain;

namespace RPM_Project_Backend.Mappings;

public class PaymentMappingProfile : Profile
{
    public PaymentMappingProfile()
    {
        CreateMap<PaymentCreateDto, Payment>().ReverseMap();
    }
}