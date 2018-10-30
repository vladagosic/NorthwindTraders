using Microsoft.Extensions.Caching.Memory;
using Northwind.Application.Interfaces;
using Northwind.Application.Suppliers.Models;
using Northwind.Application.Suppliers.Services;
using Northwind.Domain.Entities;
using Northwind.Persistence;
using Northwind.Persistence.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using static Northwind.Application.Helpers.CacheHelper;

namespace Northwind.Application.Tests.Suppliers
{
	public class SuppliersServiceTest
		: TestBase, IDisposable
	{
		private readonly MemoryCache _cache;
		private readonly NorthwindDbContext _context;
		private readonly SupplierService _service;
		private static readonly Supplier _testSupplier = 
			new Supplier
			{
				CompanyName = "Cooperativa de Quesos 'Las Cabras'",
				ContactName = "Antonio del Valle Saavedra",
				ContactTitle = "Export Administrator",
				Address = "Calle del Rosal 4",
				City = "Oviedo",
				Region = "Asturias",
				PostalCode = "33007",
				Fax = "",
				Phone = "(98) 598 76 54",
				HomePage = ""
			};

		public SuppliersServiceTest()
		{
			_cache = new MemoryCache(new MemoryCacheOptions());
			_context = InitAndGetDbContext();
			_service = new SupplierService(
				new EfAsyncRepository<Supplier>(_context),
				new EfAsyncRepository<Product>(_context),
				_cache
				);
		}

		[Fact]
		public async Task TestMemoryCacheOnServiceGetAll()
		{
			var suppliersResult = await _service.GetAll();

			var cachedResponse = _cache.Get(CacheKeys.SuppliersAll);

			Assert.NotNull(cachedResponse);
			Assert.IsType<List<SupplierDto>>(cachedResponse);
			Assert.Equal(suppliersResult.Value, cachedResponse);
		}

		[Fact]
		public async Task ShouldReturnOkSupplierForExistingId()
		{
			var supplierResult = await _service.GetSupplier(_testSupplier.Id);

			Assert.NotNull(supplierResult);
			Assert.IsType<ServiceResult<SupplierDto>>(supplierResult);
			Assert.Equal(supplierResult.Status, ServiceResult.Ok(supplierResult.Value).Status);
		}

		[Fact]
		public async Task ShouldReturnNotFoundForNotExistingId()
		{
			var supplierResult = await _service.GetSupplier(10);

			Assert.NotNull(supplierResult);
			Assert.IsType<ServiceResult<SupplierDto>>(supplierResult);
			Assert.Equal(supplierResult.Status, ServiceResult.EntityNotFound<SupplierDto>().Status);
		}

		[Fact]
		public async Task ShouldReturnOkForDeleteExisting()
		{
			var supplierResult = await _service.Delete(_testSupplier.Id);

			Assert.NotNull(supplierResult);
			Assert.IsType<ServiceResult>(supplierResult);
			Assert.Equal(supplierResult.Status, ServiceResult.Ok().Status);
		}

		[Fact]
		public async Task ShouldReturnNotFoundAfterEntityDeleted()
		{
			var deleteResult = await _service.Delete(_testSupplier.Id);
			var supplierResult = await _service.GetSupplier(_testSupplier.Id);

			Assert.NotNull(supplierResult);
			Assert.IsType<ServiceResult<SupplierDto>>(supplierResult);
			Assert.Equal(supplierResult.Status, ServiceResult.EntityNotFound<SupplierDto>().Status);
		}

		private NorthwindDbContext InitAndGetDbContext()
		{
			var context = GetDbContext();

			context.Suppliers.AddRange(new[]
			{
				_testSupplier,
				new Supplier { CompanyName = "Exotic Liquids", ContactName = "Charlotte Cooper", ContactTitle = "Purchasing Manager", Address = "49 Gilbert St.", City = "London", PostalCode = "EC1 4SD", Fax = "", Phone = "(171) 555-2222", HomePage = "" },
				new Supplier { CompanyName = "New Orleans Cajun Delights", ContactName = "Shelley Burke", ContactTitle = "Order Administrator", Address = "P.O. Box 78934", City = "New Orleans", Region = "LA", PostalCode = "70117", Fax = "", Phone = "(100) 555-4822", HomePage = "#CAJUN.HTM#" },
				new Supplier { CompanyName = "Grandma Kelly's Homestead", ContactName = "Regina Murphy", ContactTitle = "Sales Representative", Address = "707 Oxford Rd.", City = "Ann Arbor", Region = "MI", PostalCode = "48104", Fax = "(313) 555-3349", Phone = "(313) 555-5735", HomePage = "" },
				new Supplier { CompanyName = "Tokyo Traders", ContactName = "Yoshi Nagase", ContactTitle = "Marketing Manager", Address = "9-8 Sekimai Musashino-shi", City = "Tokyo", PostalCode = "100", Fax = "", Phone = "(03) 3555-5011", HomePage = "" }
			});

			context.SaveChanges();

			return context;
		}

		public void Dispose()
		{
			_context.Dispose();
		}
	}
}
