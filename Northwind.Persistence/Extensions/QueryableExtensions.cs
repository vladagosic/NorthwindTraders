using Microsoft.EntityFrameworkCore;
using Northwind.Common;
using System.Linq;

namespace Northwind.Persistence.Extensions
{
	internal static class QueryableExtensions
	{
		public static IQueryable<T> WithTrackingOption<T>(this IQueryable<T> queryable, TrackingOption tracking)
			where T : class
		{
			return tracking == TrackingOption.WithTracking
				? queryable.AsTracking()
				: queryable.AsNoTracking();
		}
	}
}
