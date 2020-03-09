using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Model
{
	public class BankPaymentResponseDto
	{
		public Guid PaymentId { get; set; }

		public DateTime ProcessingDate { get; set; }

		public bool IsProcessed { get; set; }
	}
}
