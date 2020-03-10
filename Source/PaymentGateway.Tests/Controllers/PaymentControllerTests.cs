using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PaymentGateway.Controllers;
using PaymentGateway.DataModel;
using PaymentGateway.Model;
using PaymentGateway.Services;
using PaymentGateway.Tests.Helpers;
using Xunit;

namespace PaymentGateway.Tests.Controllers
{
	public class PaymentControllerTests : TestBase
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
			using (var mock = GetMockWithAutoMapper())
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

		[Fact]
		public async Task ProcessPaymentReturnsBadRequestWhenModelStateIsInvalid()
		{
			using (var mock = AutoMock.GetLoose())
			{
				//Arrange
				var controller = mock.Create<PaymentsController>();
				var processPaymentDto = PaymentHelper.GetCorrectProcessPaymentDto();
				controller.ModelState.AddModelError("Some key", "Some message");

				//Act
				var result = await controller.ProcessPayment(processPaymentDto);

				//Assert
				Assert.IsType<BadRequestObjectResult>(result.Result);

				var badRequest = result.Result as BadRequestObjectResult;
				var error = badRequest?.Value as SerializableError;

				Assert.NotNull(error);
				Assert.Single(error);
				Assert.Equal("Some key", error.First().Key);

				var value = error.First().Value as string[];
				Assert.NotNull(value);
				Assert.Equal("Some message", value[0]);
			}
		}

		[Fact]
		public async Task ProcessPaymentReturnsBadRequestWhenCardIsExpired()
		{
			using (var mock = AutoMock.GetLoose())
			{
				//Arrange
				var controller = mock.Create<PaymentsController>();
				var processPaymentDto = PaymentHelper.GetCorrectProcessPaymentDto();
				processPaymentDto.CardExpirationMonth = "01";
				processPaymentDto.CardExpirationYear = (DateTime.UtcNow.Year - 1).ToString().Substring(2, 2);

				//Act
				var result = await controller.ProcessPayment(processPaymentDto);

				//Assert
				Assert.IsType<BadRequestObjectResult>(result.Result);

				var badRequest = result.Result as BadRequestObjectResult;
				Assert.Equal("Card is already expired", badRequest?.Value.ToString());
			}
		}

		[Fact]
		public async Task ProcessPaymentReturnsCreatedResponseWhenRequestIsValid()
		{
			using (var mock = GetMockWithAutoMapper())
			{
				//Arrange
				mock.Mock<IBankService>().Setup(x => x.ProcessPaymentRequest(It.IsAny<BankPaymentRequestDto>()))
					.Returns(() => Task.FromResult(new BankPaymentResponseDto()));

				mock.Mock<IPaymentService>().Setup(x => x.CreatePayment(It.IsAny<Payment>()))
					.Callback((Payment payment) =>
					{
						payment.Id = 1;
					})
					.Returns(() => Task.FromResult(true));

				mock.Mock<IUrlHelper>().Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>()))
					.Returns(() => "/api/payments/1");

				var controller = mock.Create<PaymentsController>();
				controller.Url = mock.Create<IUrlHelper>();

				var processPaymentDto = PaymentHelper.GetCorrectProcessPaymentDto();

				//Act
				var result = await controller.ProcessPayment(processPaymentDto);

				//Assert
				Assert.IsType<CreatedResult>(result.Result);

				var createdResult = result.Result as CreatedResult;
				Assert.Equal("/api/payments/1", createdResult?.Location);

				var paymentDetailsDto = createdResult?.Value as PaymentDetailsDto;
				Assert.NotNull(paymentDetailsDto);
				Assert.Equal(1, paymentDetailsDto.Id);

				var expectedCardNumber = (processPaymentDto.CardNumber.Substring(0, 6) + "XXXXXX" +
				                          processPaymentDto.CardNumber.Substring(12, 4));
				Assert.Equal(expectedCardNumber, paymentDetailsDto.CardNumber);

				Assert.Equal(processPaymentDto.Amount, paymentDetailsDto.Amount);
				Assert.Equal(processPaymentDto.Currency, paymentDetailsDto.Currency);
			}
		}

		[Fact]
		public async Task ProcessPaymentAssignsDataFromBankRequest()
		{
			using (var mock = GetMockWithAutoMapper())
			{
				//Arrange
				var expectedBankResponse = new BankPaymentResponseDto()
				{
					IsProcessed = true,
					ProcessingId = Guid.NewGuid(),
					ProcessingDate = DateTime.UtcNow
				};

				mock.Mock<IBankService>().Setup(x => x.ProcessPaymentRequest(It.IsAny<BankPaymentRequestDto>()))
					.Returns(() => Task.FromResult(expectedBankResponse));

				mock.Mock<IPaymentService>().Setup(x => x.CreatePayment(It.IsAny<Payment>()))
					.Returns(() => Task.FromResult(true));

				mock.Mock<IUrlHelper>().Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>()))
					.Returns(() => "/api/payments/1");

				var controller = mock.Create<PaymentsController>();
				controller.Url = mock.Create<IUrlHelper>();

				var processPaymentDto = PaymentHelper.GetCorrectProcessPaymentDto();

				//Act
				var result = await controller.ProcessPayment(processPaymentDto);

				//Assert
				Assert.IsType<CreatedResult>(result.Result);

				var createdResult = result.Result as CreatedResult;
				Assert.Equal("/api/payments/1", createdResult?.Location);

				var paymentDetailsDto = createdResult?.Value as PaymentDetailsDto;
				Assert.NotNull(paymentDetailsDto);
				Assert.Equal(
					expectedBankResponse.IsProcessed ? PaymentProcessingStatus.Success : PaymentProcessingStatus.Failed,
					paymentDetailsDto.ProcessingStatus);
				Assert.Equal(expectedBankResponse.ProcessingId, paymentDetailsDto.ProcessingId);
				Assert.Equal(expectedBankResponse.ProcessingDate, paymentDetailsDto.ProcessingDate);
			}
		}

		[Fact]
		public async Task ProcessPaymentReturnsInternalServerErrorWhenBankServiceReturnedNull()
		{
			using (var mock = GetMockWithAutoMapper())
			{
				//Arrange
				mock.Mock<IBankService>().Setup(x => x.ProcessPaymentRequest(It.IsAny<BankPaymentRequestDto>()))
					.Returns(() => Task.FromResult<BankPaymentResponseDto>(null));

				var controller = mock.Create<PaymentsController>();
				var processPaymentDto = PaymentHelper.GetCorrectProcessPaymentDto();

				//Act
				var result = await controller.ProcessPayment(processPaymentDto);

				//Assert
				Assert.IsType<ObjectResult>(result.Result);

				var statusCodeResult = result.Result as ObjectResult;
				Assert.Equal(500, statusCodeResult?.StatusCode);
				Assert.Equal("Bank processing returned empty response", statusCodeResult?.Value);
			}
		}

		[Fact]
		public async Task ProcessPaymentReturnsInternalServerErrorWhenPaymentHasNotBeenSaved()
		{
			using (var mock = GetMockWithAutoMapper())
			{
				//Arrange
				mock.Mock<IBankService>().Setup(x => x.ProcessPaymentRequest(It.IsAny<BankPaymentRequestDto>()))
					.Returns(() => Task.FromResult(new BankPaymentResponseDto()));

				mock.Mock<IPaymentService>().Setup(x => x.CreatePayment(It.IsAny<Payment>()))
					.Returns(() => Task.FromResult(false));

				var controller = mock.Create<PaymentsController>();
				var processPaymentDto = PaymentHelper.GetCorrectProcessPaymentDto();

				//Act
				var result = await controller.ProcessPayment(processPaymentDto);

				//Assert
				Assert.IsType<ObjectResult>(result.Result);

				var statusCodeResult = result.Result as ObjectResult;
				Assert.Equal(500, statusCodeResult?.StatusCode);
				Assert.Equal("Payment creation failed", statusCodeResult?.Value);
			}
		}
	}
}
