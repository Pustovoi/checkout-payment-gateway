namespace PaymentGateway.Model
{
	public class BankPaymentRequestDto
	{
		public string CardNumber { get; set; }

		public int CardExpirationMonth { get; set; }

		public int CardExpirationYear { get; set; }

		public string CardHolder { get; set; }

		public string CardCvv { get; set; }

		public decimal Amount { get; set; }

		public string Currency { get; set; }
	}
}
