using AutoMapper;
using PaymentGateway.DataModel;
using PaymentGateway.Model;

namespace PaymentGateway.Services
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<ProcessPaymentDto, Payment>()
				.ForMember(dest => dest.CardNumber, 
					m => m.MapFrom(source => source.CardNumber.Substring(0, 6) + "XXXXXX" + source.CardNumber.Substring(12, 4)));

			CreateMap<ProcessPaymentDto, BankPaymentRequestDto>();

			CreateMap<Payment, PaymentDetailsDto>();
		}
	}
}
