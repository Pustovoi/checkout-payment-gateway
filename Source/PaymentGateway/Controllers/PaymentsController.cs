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
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
	    private readonly IPaymentService _paymentService;
	    private readonly IBankService _bankService;
	    private readonly IMapper _mapper;

	    public PaymentsController(
			IPaymentService paymentService,
			IBankService bankService,
			IMapper mapper)
	    {
		    _paymentService = paymentService;
		    _bankService = bankService;
		    _mapper = mapper;
	    }

	    // GET: api/Payments/5
        [HttpGet("{id}")]
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

        // POST: api/Payments
        [HttpPost]
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

	        var bankRequest = _mapper.Map<BankPaymentRequestDto>(processPaymentDto);
	        var bankResponse = await _bankService.ProcessPaymentRequest(bankRequest);

			var payment = _mapper.Map<Payment>(processPaymentDto);

			UpdatePaymentByBankResponse(payment, bankResponse);

			var success = await _paymentService.CreatePayment(payment);

	        if (success)
	        {
		        return Created(
			        new Uri($"{HttpContext.Request.Path}/{payment.Id.ToString()}", UriKind.Relative),
			        _mapper.Map<PaymentDetailsDto>(payment));
	        }

	        return StatusCode(500);
        }

        private void UpdatePaymentByBankResponse(Payment payment, BankPaymentResponseDto bankResponse)
        {
	        payment.ProcessingId = bankResponse.PaymentId;
	        payment.ProcessingDate = bankResponse.ProcessingDate;
	        payment.ProcessingStatus = bankResponse.IsProcessed
		        ? PaymentProcessingStatus.Success
		        : PaymentProcessingStatus.Failed;
		}
    }
}
