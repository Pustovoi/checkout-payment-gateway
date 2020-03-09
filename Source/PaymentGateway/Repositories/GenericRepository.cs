using System.Linq;
using System.Threading.Tasks;
using PaymentGateway.DataModel;

namespace PaymentGateway.Repositories
{
	public class GenericRepository<TEntity> : IGenericRepository<TEntity>
		where TEntity : BaseEntity
	{
		public GenericRepository(PaymentGatewayDataContext context)
		{
			Context = context;
		}

		public TEntity GetById(int id)
		{
			return Context.Set<TEntity>().SingleOrDefault(e => e.Id == id);
		}

		public void Add(TEntity entity)
		{
			Context.Set<TEntity>().Add(entity);
		}

		public async Task<bool> SaveAsync()
		{
			return await Context.SaveChangesAsync() > 0;
		}

		protected PaymentGatewayDataContext Context { get; }
	}
}
