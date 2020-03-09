using System;

namespace PaymentGateway.DataModel
{
	public class Payment : BaseEntity
	{
		public string CardNumber { get; set; }

		public int CardExpirationMonth { get; set; }

		public int CardExpirationYear { get; set; }

		public string CardHolder { get; set; }

		public decimal Amount { get; set; }

		public string Currency { get; set; }

		public Guid ProcessingId { get; set; }

		public PaymentProcessingStatus ProcessingStatus { get; set; }

		public DateTime ProcessingDate { get; set; }
	}
}
