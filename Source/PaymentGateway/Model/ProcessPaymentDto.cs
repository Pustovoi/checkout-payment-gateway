using System.ComponentModel.DataAnnotations;
using PaymentGateway.Validators;

namespace PaymentGateway.Model
{
	public class ProcessPaymentDto
	{
		[Required(AllowEmptyStrings = false, ErrorMessage = "Card number is not specified")]
		[CreditCard(ErrorMessage = "Invalid credit card number")]
		public string CardNumber { get; set; }

		[Required(AllowEmptyStrings = false, ErrorMessage = "Card expiration month is not specified")]
		[RegularExpression(@"^0[1-9]|1[0-2]$", ErrorMessage = "Invalid card expiration month")]
		public string CardExpirationMonth { get; set; }

		//Comment: let's assume that year provided in 2 digits format

		[Required(AllowEmptyStrings = false, ErrorMessage = "Card expiration year is not specified")]
		[RegularExpression(@"^\d{2}$", ErrorMessage = "Invalid card expiration year")]
		public string CardExpirationYear { get; set; }

		public string CardHolder { get; set; }

		[Required(ErrorMessage = "Card CVV code is not specified")]
		[RegularExpression(@"^\d{3}$", ErrorMessage = "Invalid card CVV code")]
		public string CardCvv { get; set; }

		[PositiveDecimal(ErrorMessage = "Amount of payment should be greater than 0")]
		public decimal Amount { get; set; }

		[Required(AllowEmptyStrings = false, ErrorMessage = "Currency is not specified")]
		public string Currency { get; set; }
	}
}
