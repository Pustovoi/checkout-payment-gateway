using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PaymentGateway.DataModel;

namespace PaymentGateway.Services
{
	public interface IPaymentService
	{
		Payment GetPayment(int id);

		Task<bool> CreatePayment(Payment payment);
	}
}
