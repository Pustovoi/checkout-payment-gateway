using System.Threading.Tasks;
using AutoMapper;
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
	    private readonly IMapper _mapper;

	    public PaymentsController(IPaymentService paymentService, IMapper mapper)
	    {
		    _paymentService = paymentService;
		    _mapper = mapper;
	    }

	    // GET: api/Payments/5
        [HttpGet("{id}")]
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
        public async Task<ActionResult<PaymentDetailsDto>> ProcessPayment([FromBody] ProcessPaymentDto processPaymentDto)
        {
	        if (!ModelState.IsValid)
	        {
		        return BadRequest(ModelState);
	        }

	        var payment = _mapper.Map<Payment>(processPaymentDto);

	        var success = await _paymentService.CreatePayment(payment);

	        if (success)
	        {
		        return Created("", _mapper.Map<PaymentDetailsDto>(payment));
			}

	        return StatusCode(500);
        }
    }
}
