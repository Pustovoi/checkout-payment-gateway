using System;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extras.Moq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PaymentGateway.Controllers;
using PaymentGateway.DataModel;
using PaymentGateway.Model;
using PaymentGateway.Services;
using Xunit;

namespace PaymentGateway.Tests.Controllers
{
	public class PaymentControllerTests
	{
		[Fact]
		public void GetPaymentDetailsReturnsNotFoundWhenPaymentDoesNotExist()
		{
			using (var mock = AutoMock.GetLoose())
			{
				//Arrange
				mock.Mock<IPaymentService>().Setup(x => x.GetPayment(It.IsAny<int>())).Returns(() => null);
				var controller = mock.Create<PaymentsController>();

				//Act
				var result = controller.GetPaymentDetails(1);

				//Assert
				Assert.IsType<NotFoundResult>(result.Result);
			}
		}

		[Fact]
		public void GetPaymentDetailsReturnsPaymentDetails()
		{
			using (var mock = GetMockWithAutomapper())
			{
				//Arrange
				var payment = new Payment()
				{
					Id = 1,
					CardNumber = "1234",
					Amount = 20,
					Currency = "EUR",
					ProcessingStatus = PaymentProcessingStatus.Success,
					ProcessingDate = DateTime.UtcNow,
					ProcessingId = Guid.NewGuid()
				};


				mock.Mock<IPaymentService>().Setup(x => x.GetPayment(It.IsAny<int>())).Returns(() => payment);
				var controller = mock.Create<PaymentsController>();

				//Act
				var result = controller.GetPaymentDetails(1);

				//Assert
				Assert.IsType<OkObjectResult>(result.Result);

				var value = ((OkObjectResult)result.Result).Value as PaymentDetailsDto;

				Assert.NotNull(value);

				Assert.Equal(payment.Id, value.Id);
				Assert.Equal(payment.CardNumber, value.CardNumber);
				Assert.Equal(payment.Amount, value.Amount);
				Assert.Equal(payment.Currency, value.Currency);
				Assert.Equal(payment.ProcessingStatus, value.ProcessingStatus);
				Assert.Equal(payment.ProcessingDate, value.ProcessingDate);
				Assert.Equal(payment.ProcessingId, value.ProcessingId);
			}
		}

		[Fact]
		public async Task ProcessPaymentReturnsBadRequestWhenPaymentIsNull()
		{
			using (var mock = AutoMock.GetLoose())
			{
				//Arrange
				var controller = mock.Create<PaymentsController>();

				//Act
				var result = await controller.ProcessPayment(null);

				//Assert
				Assert.IsType<BadRequestResult>(result.Result);
			}
		}

		private AutoMock GetMockWithAutomapper()
		{
			return AutoMock.GetLoose((builder) =>
			{
				builder.Register(
						c => new MapperConfiguration(cfg => { cfg.AddProfiles(new[] {new MappingProfile()}); }))
					.AsSelf()
					.SingleInstance();

				//register  mapper
				builder.Register(
						c => c.Resolve<MapperConfiguration>().CreateMapper(c.Resolve))
					.As<IMapper>().InstancePerLifetimeScope();
			});
		}
	}
}
