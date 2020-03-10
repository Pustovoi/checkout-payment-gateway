using System;
using System.Collections.Generic;
using System.Text;
using PaymentGateway.Validators;
using Xunit;

namespace PaymentGateway.Tests.Validators
{
	public class PositiveDecimalAttributeTests
	{
		[Fact]
		public void IsInvalidWhenParameterIsNotDecimal()
		{
			//Arrange
			var attribute = new PositiveDecimalAttribute();

			//Act
			var result = attribute.IsValid(5);

			//Assert
			Assert.False(result);
		}

		[Fact]
		public void IsInvalidWhenNegative()
		{
			//Arrange
			var attribute = new PositiveDecimalAttribute();

			//Act
			var result = attribute.IsValid(-5M);

			//Assert
			Assert.False(result);
		}

		[Fact]
		public void IsInvalidWhenZero()
		{
			//Arrange
			var attribute = new PositiveDecimalAttribute();

			//Act
			var result = attribute.IsValid(0M);

			//Assert
			Assert.False(result);
		}

		[Fact]
		public void IsInvalidWhenPositive()
		{
			//Arrange
			var attribute = new PositiveDecimalAttribute();

			//Act
			var result = attribute.IsValid(5M);

			//Assert
			Assert.True(result);
		}
	}
}
