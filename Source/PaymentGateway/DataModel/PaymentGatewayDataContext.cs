using Microsoft.EntityFrameworkCore;

namespace PaymentGateway.DataModel
{
	public class PaymentGatewayDataContext : DbContext
	{
		public DbSet<Payment> Payments { get; set; }
	}
}
