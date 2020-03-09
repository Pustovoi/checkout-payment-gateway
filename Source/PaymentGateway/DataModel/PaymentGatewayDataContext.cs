using Microsoft.EntityFrameworkCore;

namespace PaymentGateway.DataModel
{
	public class PaymentGatewayDataContext : DbContext
	{
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);

			optionsBuilder.UseInMemoryDatabase("PaymentsDb");
		}

		public DbSet<Payment> Payments { get; set; }
	}
}
