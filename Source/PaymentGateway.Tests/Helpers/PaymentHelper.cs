using System;
using System.Collections.Generic;
using System.Text;
using PaymentGateway.Model;

namespace PaymentGateway.Tests.Helpers
{
	public static class PaymentHelper
	{
		public static ProcessPaymentDto GetCorrectProcessPaymentDto()
		{
			return new ProcessPaymentDto()
			{
				CardNumber = "4444333322221111",
				CardExpirationMonth = "10",
				CardExpirationYear = "25",
				CardCvv = "123",
				CardHolder = "Card Holder",
				Amount = 30,
				Currency = "EUR"
			};
		}
	}
}
