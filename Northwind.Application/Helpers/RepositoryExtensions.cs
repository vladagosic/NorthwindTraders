﻿using Northwind.Application.Interfaces;
using Northwind.Common;
using Northwind.Domain.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Northwind.Application.Helpers
{
	public static class RepositoryExtensions
	{
		public static async Task<IEnumerable<T>> GetAll<T>(this IAsyncRepository<T> repository, Expression<Func<T, bool>> criteria)
		{
			return await repository.GetAll(new Specification<T>(criteria));
		}

		public static async Task<T> GetByIdAndInclude<T>(this IAsyncRepository<T> repository, int id, Expression<Func<T, object>> include, TrackingOption tracking = TrackingOption.WithTracking)
			where T : Entity
		{
			return (await repository.GetAll(new Specification<T>(
					t => t.Id == id,
					new[] { include },
					null,
					tracking
				)))
				.FirstOrDefault();
		}
	}
}
