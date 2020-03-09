using System;
using PaymentGateway.DataModel;

namespace PaymentGateway.Model
{
	public class PaymentDetailsDto
	{
		//Comment: Currently dto information matches stored in DB, but in general case if I needed to store sensitive information it would be different

		public int Id { get; set; }

		public string CardNumber { get; set; }

		public decimal Amount { get; set; }

		public string Currency { get; set; }

		public Guid ProcessingId { get; set; }

		public PaymentProcessingStatus ProcessingStatus { get; set; }

		public DateTime ProcessingDate { get; set; }
	}
}
