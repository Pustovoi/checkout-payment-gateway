using System.Threading.Tasks;
using PaymentGateway.DataModel;
using PaymentGateway.Repositories;

namespace PaymentGateway.Services
{
	public class PaymentService : IPaymentService
	{
		private readonly IPaymentRepository _repository;

		public PaymentService(IPaymentRepository repository)
		{
			_repository = repository;
		}

		public Payment GetPayment(int id)
		{
			return _repository.GetById(id);
		}

		public async Task<bool> CreatePayment(Payment payment)
		{
			_repository.Add(payment);

			return await _repository.SaveAsync();
		}
	}
}
