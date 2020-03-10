using System;

namespace PaymentGateway.Model
{
	public class BankPaymentResponseDto
	{
		public Guid ProcessingId { get; set; }

		public DateTime ProcessingDate { get; set; }

		public bool IsProcessed { get; set; }
	}
}
