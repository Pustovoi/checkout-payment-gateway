using System.Threading.Tasks;

namespace PaymentGateway.Repositories
{
	public interface IGenericRepository<TEntity>
	{
		TEntity GetById(int id);

		void Add(TEntity entity);

		Task<bool> SaveAsync();
	}
}
