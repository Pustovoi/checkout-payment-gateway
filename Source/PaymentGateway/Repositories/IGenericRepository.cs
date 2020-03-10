using System.Threading.Tasks;

namespace PaymentGateway.Repositories
{
	public interface IGenericRepository<TEntity>
	{
		//Comment: It is not full implementation of generic repository. Only necessary for the task methods implemented

		TEntity GetById(int id);

		void Add(TEntity entity);

		Task<bool> SaveAsync();
	}
}
