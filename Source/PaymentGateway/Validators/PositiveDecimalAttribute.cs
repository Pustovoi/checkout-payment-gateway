using System.ComponentModel.DataAnnotations;

namespace PaymentGateway.Validators
{
	public class PositiveDecimalAttribute : ValidationAttribute
	{
		public override bool IsValid(object value)
		{
			if (value is decimal)
			{
				var decimalValue = (decimal) value;

				return decimalValue > 0;
			}

			return false;
		}
	}
}
