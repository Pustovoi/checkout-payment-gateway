using System.Threading.Tasks;
using AutoMapper;
using PaymentGateway.DataModel;
using PaymentGateway.Model;
using PaymentGateway.Proxies;
using PaymentGateway.Repositories;

namespace PaymentGateway.Services
{
	public class PaymentService : IPaymentService
	{
		private readonly IPaymentRepository _repository;
		private readonly IBankProxy _bankProxy;
		private readonly IMapper _mapper;

		public PaymentService(IPaymentRepository repository, IBankProxy bankProxy, IMapper mapper)
		{
			_repository = repository;
			_bankProxy = bankProxy;
			_mapper = mapper;
		}

		public Payment GetPayment(int id)
		{
			return _repository.GetById(id);
		}

		public async Task<bool> CreatePayment(Payment payment)
		{
			var bankRequest = _mapper.Map<BankPaymentRequestDto>(payment);

			var bankResponse = await _bankProxy.ProcessPaymentRequest(bankRequest);

			payment.ProcessingId = bankResponse.PaymentId;
			payment.ProcessingDate = bankResponse.ProcessingDate;
			payment.ProcessingStatus = bankResponse.IsProcessed
				? PaymentProcessingStatus.Success
				: PaymentProcessingStatus.Failed;

			_repository.Add(payment);

			return await _repository.SaveAsync();
		}
	}
}
