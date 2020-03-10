using System.Threading.Tasks;
using PaymentGateway.Model;

namespace PaymentGateway.Services
{
	/// <summary>
	/// Represents an interface of bank service
	/// </summary>
	public interface IBankService
	{
		/// <summary>
		/// Processes payment
		/// </summary>
		/// <param name="paymentRequestDto">Payment data</param>
		/// <returns>Processing result</returns>
		Task<BankPaymentResponseDto> ProcessPaymentRequest(BankPaymentRequestDto paymentRequestDto);
	}
}
