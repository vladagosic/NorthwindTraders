using Northwind.Persistence.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Northwind.Persistence.Interfaces
{
	/// <summary>
	/// A generic repository of entities of type <typeparamref name="T"/>
	/// that enables asynchronous access.
	/// </summary>
	/// <remarks>
	/// To learn more about the Repository Pattern see:
	/// https://martinfowler.com/eaaCatalog/repository.html
	/// To learn more about asynchronous programming and async-await see:
	/// https://en.wikipedia.org/wiki/Async/await
	/// </remarks>
	public interface IAsyncRepository<T>
	{
		/// <summary>
		/// Gets an entity by id or null if such entity does not exist.
		/// </summary>
		Task<T> GetById(int id);
		/// <summary>
		/// Gets the first entity that satisfies the <paramref name="specification"/>
		/// or null if such entity does not exist.
		/// </summary>
		Task<T> GetFirst(ISpecification<T> specification);
		/// <summary>
		/// Gets all entities or empty enumerable if the repository does not contain
		/// any entity.
		/// </summary>
		Task<IEnumerable<T>> GetAll(TrackingOption tracking = TrackingOption.WithTracking);
		/// <summary>
		/// Gets all entities that satisfy the <paramref name="specification"/>
		/// or empty enumerable if such entities do not exist.
		/// </summary>
		Task<IEnumerable<T>> GetAll(ISpecification<T> specification);
		/// <summary>
		/// Adds a new <paramref name="entity"/> in the repository.
		/// </summary>
		Task<T> Add(T entity);
		/// <summary>
		/// Updates an existing <paramref name="entity"/> in the repository.
		/// </summary>
		Task<int> Update(T entity);
		/// <summary>
		/// Deletes an existing <paramref name="entity"/> form the repository.
		/// </summary>
		Task Delete(T entity);
	}
}
