using System;
using System.Net.Mime;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.DataModel;
using PaymentGateway.Model;
using PaymentGateway.Services;

namespace PaymentGateway.Controllers
{
	/// <summary>
	/// Represents payment controller
	/// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
	    private readonly IPaymentService _paymentService;
	    private readonly IBankService _bankService;
	    private readonly IMapper _mapper;

		/// <summary>
		/// Constructor
		/// </summary>
		public PaymentsController(
			IPaymentService paymentService,
			IBankService bankService,
			IMapper mapper)
	    {
		    _paymentService = paymentService;
		    _bankService = bankService;
		    _mapper = mapper;
	    }

		/// <summary>
		/// Gets payment details by id. Example: GET api/payments/5
		/// </summary>
		/// <param name="id">Payment identifier</param>
		/// <returns>Payment details</returns>
        [HttpGet("{id}", Name = "GetPaymentDetails")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult<PaymentDetailsDto> GetPaymentDetails(int id)
        {
	        var payment = _paymentService.GetPayment(id);

	        if (payment == null)
	        {
		        return NotFound();
	        }
			
            return Ok(_mapper.Map<PaymentDetailsDto>(payment));
        }

		/// <summary>
		/// Processes payment and stores it in the DB. Example: POST api/payments
		/// </summary>
		/// <param name="processPaymentDto">Payment data</param>
		/// <returns>Processed payment details</returns>
		[HttpPost(Name = "ProcessPayment")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<PaymentDetailsDto>> ProcessPayment([FromBody] ProcessPaymentDto processPaymentDto)
        {
	        if (processPaymentDto == null)
	        {
		        return BadRequest();
	        }

	        if (!ModelState.IsValid)
	        {
		        return BadRequest(ModelState);
	        }

	        if (!IsExpirationDateActual(processPaymentDto.CardExpirationMonth, processPaymentDto.CardExpirationYear))
	        {
		        return BadRequest("Card is already expired");
	        }

	        var bankRequest = _mapper.Map<BankPaymentRequestDto>(processPaymentDto);
	        var bankResponse = await _bankService.ProcessPaymentRequest(bankRequest);

	        if (bankResponse == null)
	        {
		        return StatusCode(500, "Bank processing returned empty response");
			}

			var payment = _mapper.Map<Payment>(processPaymentDto);

			UpdatePaymentByBankResponse(payment, bankResponse);

			var success = await _paymentService.CreatePayment(payment);

	        if (success)
	        {
		        return Created(
					Url.Link("GetPaymentDetails", new { id = payment.Id }),
			        _mapper.Map<PaymentDetailsDto>(payment));
	        }

	        return StatusCode(500, "Payment creation failed");
        }

        private void UpdatePaymentByBankResponse(Payment payment, BankPaymentResponseDto bankResponse)
        {
	        payment.ProcessingId = bankResponse.ProcessingId;
	        payment.ProcessingDate = bankResponse.ProcessingDate;
	        payment.ProcessingStatus = bankResponse.IsProcessed
		        ? PaymentProcessingStatus.Success
		        : PaymentProcessingStatus.Failed;
		}

        private bool IsExpirationDateActual(string expirationMonth, string expirationYear)
        {
	        try
	        {
		        var expirationDate = new DateTime(int.Parse("20" + expirationYear), int.Parse(expirationMonth), 1);

		        expirationDate = expirationDate.AddMonths(1);

		        //I am not taking into account time zones here.
		        return DateTime.UtcNow < expirationDate;
			}
	        catch (ArgumentOutOfRangeException)
	        {
		        return false;
	        }
        }
    }
}
