using System;
using System.Threading.Tasks;
using PaymentGateway.Model;

namespace PaymentGateway.Services
{
	public class BankServiceStub : IBankService
	{
		public Task<BankPaymentResponseDto> ProcessPaymentRequest(BankPaymentRequestDto paymentRequestDto)
		{
			return Task.Run(() =>
			{
				//Let's imagine that this bank processes Visa only
				if (paymentRequestDto.CardNumber.StartsWith("4"))
				{
					return new BankPaymentResponseDto()
					{
						IsProcessed = true,
						ProcessingId = Guid.NewGuid(),
						ProcessingDate = DateTime.UtcNow
					};
				}

				return new BankPaymentResponseDto()
				{
					IsProcessed = false
				};
			});
		}
	}
}
