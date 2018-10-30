using Northwind.Persistence;

namespace Northwind.Application.Tests.Infrastructure
{
	public class ServiceTestBase
	{
		protected readonly NorthwindDbContext _context;

		public ServiceTestBase()
		{
			_context = NorthwindContextFactory.Create();
		}

		public void Dispose()
		{
			NorthwindContextFactory.Destroy(_context);
		}
	}
}
