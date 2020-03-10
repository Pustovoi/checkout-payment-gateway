using System;

namespace PaymentGateway.DataModel
{
	public class Payment : BaseEntity
	{
		//Comment: I decided to do not store all details and limit data by details related to card
		//and amount of payment to do not store sensitive information Which can identify payer
		//Currency is string, in real case it will be another table

		public string CardNumber { get; set; }

		public decimal Amount { get; set; }

		public string Currency { get; set; }

		public Guid ProcessingId { get; set; }

		public PaymentProcessingStatus ProcessingStatus { get; set; }

		public DateTime ProcessingDate { get; set; }
	}
}
