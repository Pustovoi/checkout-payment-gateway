namespace PaymentGateway.Model
{
	public class BankPaymentRequestDto
	{
		public string CardNumber { get; set; }

		public string CardExpirationMonth { get; set; }

		public string CardExpirationYear { get; set; }

		public string CardHolder { get; set; }

		public string CardCvv { get; set; }

		public decimal Amount { get; set; }

		public string Currency { get; set; }
	}
}
