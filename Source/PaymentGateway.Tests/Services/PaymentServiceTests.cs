using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using Moq;
using PaymentGateway.DataModel;
using PaymentGateway.Repositories;
using PaymentGateway.Services;
using Xunit;

namespace PaymentGateway.Tests.Services
{
	public class PaymentServiceTests
	{
		[Fact]
		public void GetPaymentReturnsResultFromRepository()
		{
			using (var mock = AutoMock.GetLoose())
			{
				//Arrange
				var expectedPayment = new Payment();

				mock.Mock<IPaymentRepository>()
					.Setup(x => x.GetById(It.IsAny<int>()))
					.Returns(() => expectedPayment);

				var service = mock.Create<PaymentService>();

				//Act
				var payment = service.GetPayment(1);

				//Assert
				Assert.NotNull(payment);
				Assert.Equal(expectedPayment, payment);
			}
		}

		[Fact]
		public async Task CreatePaymentAddsPaymentAndSavesResult()
		{
			using (var mock = AutoMock.GetLoose())
			{
				//Arrange
				var expectedPayment = new Payment();
				var addedPayments = new List<Payment>();

				mock.Mock<IPaymentRepository>()
					.Setup(x => x.Add(It.IsAny<Payment>()))
					.Callback((Payment p) =>
					{
						addedPayments.Add(p);
					});

				mock.Mock<IPaymentRepository>()
					.Setup(x => x.SaveAsync())
					.Returns(() => Task.FromResult(true));

				var service = mock.Create<PaymentService>();

				//Act
				var success = await service.CreatePayment(expectedPayment);

				//Assert
				Assert.True(success);
				Assert.Single(addedPayments);
				Assert.Equal(expectedPayment, addedPayments.First());
			}
		}
	}
}
