using PaymentGateway.DataModel;

namespace PaymentGateway.Repositories
{
	public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
	{
		public PaymentRepository(PaymentGatewayDataContext context) : base(context)
		{
		}
	}
}
