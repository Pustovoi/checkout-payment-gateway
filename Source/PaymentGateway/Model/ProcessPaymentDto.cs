using System.ComponentModel.DataAnnotations;

namespace PaymentGateway.Model
{
	public class ProcessPaymentDto
	{
		[Required(ErrorMessage = "Card number is not specified")]
		[CreditCard(ErrorMessage = "Invalid credit card number")]
		public string CardNumber { get; set; }

		[Required(ErrorMessage = "Card expiration month is not specified")]
		[Range(1, 12, ErrorMessage = "Card expiration month should be between 1 and 12")]
		public int CardExpirationMonth { get; set; }

		[Required(ErrorMessage = "Card expiration year is not specified")]
		public int CardExpirationYear { get; set; }

		public string CardHolder { get; set; }

		[Required(ErrorMessage = "Card CVV code is not specified")]
		[StringLength(
			maximumLength: 3,
			MinimumLength = 3,
			ErrorMessage = "Card cvv code should contain exactly 3 symbols")]
		[RegularExpression(@"\d+", ErrorMessage = "Invalid card CVV code")]
		public string CardCvv { get; set; }

		public decimal Amount { get; set; }

		public string Currency { get; set; }
	}
}
