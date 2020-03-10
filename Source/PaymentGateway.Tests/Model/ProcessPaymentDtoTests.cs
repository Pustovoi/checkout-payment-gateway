using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using PaymentGateway.Model;
using PaymentGateway.Tests.Helpers;
using Xunit;

namespace PaymentGateway.Tests.Model
{
	public class ProcessPaymentDtoTests : TestBase
	{
		[Fact]
		public void ReturnsErrorWhenCardNumberIsNotSpecified()
		{
			var payment = PaymentHelper.GetCorrectProcessPaymentDto();
			payment.CardNumber = string.Empty;

			SingleErrorTest(payment, "Card number is not specified");
		}

		[Fact]
		public void ReturnsErrorWhenCardNumberIsInvalid()
		{
			var payment = PaymentHelper.GetCorrectProcessPaymentDto();
			payment.CardNumber = "34575cbvxdvbf";

			SingleErrorTest(payment, "Invalid credit card number");
		}

		[Fact]
		public void ReturnsErrorWhenCardExpirationMonthIsNotSpecified()
		{
			var payment = PaymentHelper.GetCorrectProcessPaymentDto();
			payment.CardExpirationMonth = string.Empty;

			SingleErrorTest(payment, "Card expiration month is not specified");
		}

		[Fact]
		public void ReturnsErrorWhenCardExpirationMonthIsInvalid()
		{
			var payment = PaymentHelper.GetCorrectProcessPaymentDto();
			payment.CardExpirationMonth = "18";

			SingleErrorTest(payment, "Invalid card expiration month");
		}

		[Fact]
		public void ReturnsErrorWhenCardExpirationYearIsNotSpecified()
		{
			var payment = PaymentHelper.GetCorrectProcessPaymentDto();
			payment.CardExpirationYear = string.Empty;

			SingleErrorTest(payment, "Card expiration year is not specified");
		}

		[Fact]
		public void ReturnsErrorWhenCardExpirationYearIsInvalid()
		{
			var payment = PaymentHelper.GetCorrectProcessPaymentDto();
			payment.CardExpirationYear = "1a";

			SingleErrorTest(payment, "Invalid card expiration year");
		}

		[Fact]
		public void ReturnsErrorWhenCardCvvIsNotSpecified()
		{
			var payment = PaymentHelper.GetCorrectProcessPaymentDto();
			payment.CardCvv = string.Empty;

			SingleErrorTest(payment, "Card CVV code is not specified");
		}

		[Fact]
		public void ReturnsErrorWhenCardCvvIsInvalid()
		{
			var payment = PaymentHelper.GetCorrectProcessPaymentDto();
			payment.CardCvv = "abc";

			SingleErrorTest(payment, "Invalid card CVV code");
		}

		[Fact]
		public void ReturnsErrorWhenAmountIsNegative()
		{
			var payment = PaymentHelper.GetCorrectProcessPaymentDto();
			payment.Amount = -5;

			SingleErrorTest(payment, "Amount of payment should be greater than 0");
		}

		[Fact]
		public void ReturnsErrorWhenAmountIsZero()
		{
			var payment = PaymentHelper.GetCorrectProcessPaymentDto();
			payment.Amount = 0;

			SingleErrorTest(payment, "Amount of payment should be greater than 0");
		}

		[Fact]
		public void ReturnsErrorWhenCurrencyIsNotSpecified()
		{
			var payment = PaymentHelper.GetCorrectProcessPaymentDto();
			payment.Currency = string.Empty;

			SingleErrorTest(payment, "Currency is not specified");
		}

		[Fact]
		public void DoNotReturnErrorWhenPaymentInformationIsValid()
		{
			//Arrange
			var payment = PaymentHelper.GetCorrectProcessPaymentDto();
			var context = new ValidationContext(payment);
			var results = new List<ValidationResult>();

			//Act
			Validator.TryValidateObject(payment, context, results, true);

			//Assert
			Assert.Empty(results);
		}

		private void SingleErrorTest(ProcessPaymentDto payment, string expectedMessage)
		{
			//Arrange
			var context = new ValidationContext(payment);
			var results = new List<ValidationResult>();

			//Act
			Validator.TryValidateObject(payment, context, results, true);

			//Assert
			Assert.Single(results);
			Assert.Equal(expectedMessage, results.First().ErrorMessage);
		}
	}
}
