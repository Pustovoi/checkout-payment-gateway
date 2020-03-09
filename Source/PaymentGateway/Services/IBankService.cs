using System.Threading.Tasks;
using PaymentGateway.Model;

namespace PaymentGateway.Services
{
	public interface IBankService
	{
		Task<BankPaymentResponseDto> ProcessPaymentRequest(BankPaymentRequestDto paymentRequestDto);
	}
}
