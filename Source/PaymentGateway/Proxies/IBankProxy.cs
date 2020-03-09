using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PaymentGateway.Model;

namespace PaymentGateway.Proxies
{
	public interface IBankProxy
	{
		Task<BankPaymentResponseDto> ProcessPaymentRequest(BankPaymentRequestDto paymentRequestDto);
	}
}
