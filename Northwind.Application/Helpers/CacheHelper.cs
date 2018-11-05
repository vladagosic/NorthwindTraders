using Microsoft.Extensions.Caching.Memory;
using Northwind.Application.Suppliers.Models;
using Northwind.Common;
using Northwind.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Northwind.Application.Helpers
{
	public class CacheHelper
	{
		public static class CacheKeys
		{
			public const string SuppliersAll = nameof(SuppliersAll);
		}

		private readonly IAsyncRepository<Supplier> _supplierRepository;
		private readonly IMemoryCache _memoryCache;

		public CacheHelper(IMemoryCache memoryCache)
		{
			_memoryCache = memoryCache;
		}
		public CacheHelper(IAsyncRepository<Supplier> supplierRepository, IMemoryCache memoryCache)
		{
			_supplierRepository = supplierRepository;
			_memoryCache = memoryCache;
		}

		/// <summary>
		/// Get Suppliers from the DB or from the Cache if exists.
		/// </summary>
		/// <returns></returns>
		public async Task<List<SupplierDto>> GetSuppliers()
		{
			string cacheKey = CacheKeys.SuppliersAll;

			// TryGet returns true if the cache entry was found.
			// Othervise Set it into the cache
			if (!_memoryCache.TryGetValue(cacheKey, out List<SupplierDto> suppliersAll))
			{
				// Get data from the store, DB.
				suppliersAll = (await _supplierRepository
				.GetAll(TrackingOption.WithoutTracking)) // Get the entities from the repository.
				.Select(action => action.TranslateTo<SupplierDto>()) // Translate them into DTOs.
				.ToList();

				// If there is any Set to cache
				if (suppliersAll.Count() > 0)
				{
					// Store object into the cache
					_memoryCache.Set(cacheKey, suppliersAll,
						new MemoryCacheEntryOptions()
						.SetSlidingExpiration(TimeSpan.FromSeconds(15)) // After last call.
						//.SetAbsoluteExpiration(TimeSpan.FromSeconds(20)) // Absolute cache life duration.
						//.SetPriority(CacheItemPriority.High) // If memory is low cache will be cleaned, with priority we can set in which order.
						.RegisterPostEvictionCallback(CacheActionAllRemovedCallback) // Register postback method on cache key changes.
					);
				}
			}

			return suppliersAll;
		}

		/// <summary>
		/// Callback method for cache Key ActionAll events.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="reason"></param>
		/// <param name="state"></param>
		private static void CacheActionAllRemovedCallback(object key, object value, EvictionReason reason, object state)
		{
			// Do something clever on cache Key removed.
			if (reason == EvictionReason.Capacity)
			{
				// Log memory issues for cache.
			}

		}

		public void RemoveSuppliers()
		{
			_memoryCache.Remove(CacheKeys.SuppliersAll);
		}
	}
}
