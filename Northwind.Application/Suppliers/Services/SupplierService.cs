using Microsoft.Extensions.Caching.Memory;
using Northwind.Application.Helpers;
using Northwind.Application.Interfaces;
using Northwind.Application.Suppliers.Models;
using Northwind.Common;
using Northwind.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Northwind.Application.Interfaces.ServiceResult;

namespace Northwind.Application.Suppliers.Services
{
	public class SupplierService : ISupplierService
	{
		private readonly IAsyncRepository<Supplier> _supplierRepository;
		private readonly IAsyncRepository<Product> _productRepository;
		private readonly IMemoryCache _memoryCache;

		public SupplierService(
			IAsyncRepository<Supplier> supplierRepository, 
			IAsyncRepository<Product> productRepository, 
			IMemoryCache memoryCache)
		{
			_supplierRepository = supplierRepository ?? throw new ArgumentNullException(nameof(supplierRepository));
			_productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
			_memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
		}

		public async Task<ServiceResult<SupplierDto>> GetSupplier(int id)
		{
			// Get the entity from the repository.
			var supplier = await _supplierRepository.GetById(id);

			// Return the appropriate service result if the
			// requested entity does not exist.
			if (supplier == null)
				return EntityNotFound<SupplierDto>();

			// Translate it into DTO and return.
			return supplier
				.TranslateTo<SupplierDto>()
				.ToOkServiceResult();
		}

		public async Task<ServiceResult<List<SupplierDto>>> GetAll()
		{
			CacheHelper cacheHelper = new CacheHelper(_supplierRepository, _memoryCache);

			return (await cacheHelper.GetSuppliers())
				.ToOkServiceResult(); // And return.
		}

		// This is the simplest implementation usual in CRUD scenarios
		// or CRUD scenarios with additional validation.
		// In complex scenarios creating a new entity and editing existing
		// once can differ significantly. In that case we recommend to
		// split this method into two or more.
		public async Task<ServiceResult<SupplierDto>> CreateOrUpdate(SupplierDto supplierDto)
		{
			// Either create a new entity based on the DTO
			// or change an existing one based on the DTO.
			Supplier supplier = supplierDto.RepresentsNewEntity
				? supplierDto.TranslateTo<Supplier>()
				: (await _supplierRepository.GetById(supplierDto.Id)).CopyPropertiesFrom(supplierDto);

			// Check if the entity exists (if it was an update).
			if (supplier == null)
				return EntityNotFound(supplierDto);

			// TODO: Later on we will do the checks here.
			//       So far we assume everything always works fine.

			// Save changes.
			if (supplier.Id == default)
			{
				supplier = await _supplierRepository.Add(supplier);
			}			
			else
			{
				await _supplierRepository.Update(supplier);
			}
			

			// If the DTO was representing a new DTO
			// we need to set the assigned Id.
			// If it already had the Id, it is the same one.
			// So we can simply have an assignment here.

			supplierDto.Id = supplier.Id;

			// Remove SuppliersAll from cache, next time getall call will fill the cache.
			CacheHelper cacheHelper = new CacheHelper(_memoryCache);
			cacheHelper.RemoveSuppliers();

			return Ok(supplierDto);
		}

		public async Task<ServiceResult> Delete(int id)
		{
			Supplier supplier = await _supplierRepository.GetById(id);

			// Check if the entity exists.
			if (supplier == null)
				return EntityNotFound();

			// Additional business requirement: 
			// Clear the SupplierId field from Products before removing supplier
			var supplierProducts = await _productRepository.GetAll(p => p.SupplierId == id);

			supplierProducts.ToList().ForEach(async product =>
			{
				product.SupplierId = null;
				await _productRepository.Update(product);
			});

			// finally delete the Supplier
			await _supplierRepository.Delete(supplier);

			// Remove SuppliersAll from cache, next time getall call will fill the cache.
			CacheHelper cacheHelper = new CacheHelper(_memoryCache);
			cacheHelper.RemoveSuppliers();

			return Ok();
		}
	}
}
