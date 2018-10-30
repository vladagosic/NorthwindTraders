using Microsoft.EntityFrameworkCore;
using Northwind.Persistence.Extensions;
using Northwind.Persistence.Helpers;
using Northwind.Persistence.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Persistence.Infrastructure
{
	/// <summary>
	/// Entity Framework based implementation of the <see cref="IAsyncRepository{T}"/>.
	/// </summary>
	public class EfAsyncRepository<T> : IAsyncRepository<T> where T : class
	{
		protected NorthwindDbContext DbContext { get; }

		public EfAsyncRepository(NorthwindDbContext dbContext)
		{
			DbContext = dbContext;
		}

		public async Task<T> GetById(int id)
		{
			return await DbContext.Set<T>().FindAsync(id);
		}

		public async Task<T> GetFirst(ISpecification<T> specification)
		{
			return await GetQueryableForSpecification(specification).FirstOrDefaultAsync();
		}

		public async Task<IEnumerable<T>> GetAll(TrackingOption tracking = TrackingOption.WithTracking)
		{
			return await DbContext.Set<T>().WithTrackingOption(tracking).ToListAsync();
		}

		public async Task<IEnumerable<T>> GetAll(ISpecification<T> specification)
		{
			return await GetQueryableForSpecification(specification).ToListAsync();
		}

		private IQueryable<T> GetQueryableForSpecification(ISpecification<T> specification)
		{
			// Fetch a queryable that includes all expression-based includes.
			var queryableResultWithIncludes = specification
				.Includes
				.Aggregate(DbContext.Set<T>().AsQueryable(), (current, include) => current.Include(include));

			// Modify the queryable to include any string-based include statements.
			var secondaryResult = specification
				.IncludesAsStrings
				.Aggregate(queryableResultWithIncludes, (current, include) => current.Include(include));

			// Return the result of the query using the specification's criteria expression.
			return secondaryResult
				.Where(specification.Criteria)
				.WithTrackingOption(specification.Tracking);
		}

		public async Task<T> Add(T entity)
		{
			await DbContext.Set<T>().AddAsync(entity);

			await DbContext.SaveChangesAsync();

			return entity;
		}

		public async Task<int> Update(T entity)
		{
			DbContext.Entry(entity).State = EntityState.Modified;

			return await DbContext.SaveChangesAsync();
		}

		public async Task Delete(T entity)
		{
			DbContext.Set<T>().Remove(entity);
			await DbContext.SaveChangesAsync();
		}
	}
}
