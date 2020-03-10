using System.Threading.Tasks;
using PaymentGateway.DataModel;

namespace PaymentGateway.Services
{
	/// <summary>
	/// Represents an interface of payment service
	/// </summary>
	public interface IPaymentService
	{
		/// <summary>
		/// Gets payment by id
		/// </summary>
		/// <param name="id">Payment identifier</param>
		/// <returns>Payment data</returns>
		Payment GetPayment(int id);

		/// <summary>
		/// Creates payment in the data storage
		/// </summary>
		/// <param name="payment">Payment details</param>
		/// <returns>Creation status</returns>
		Task<bool> CreatePayment(Payment payment);
	}
}
