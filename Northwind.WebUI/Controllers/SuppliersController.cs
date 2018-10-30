using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Northwind.Application.Helpers;
using Northwind.Application.Interfaces;
using Northwind.Application.Suppliers.Models;
using Northwind.WebUI.Security;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Northwind.WebUI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class SuppliersController : BaseController
	{
		private static class Routes
		{
			public const string GetActionById = nameof(GetActionById);
		}

		private readonly ISupplierService _supplierService;

		public SuppliersController(ISupplierService supplierService)
		{
			_supplierService = supplierService ?? throw new ArgumentNullException(nameof(supplierService));
		}

		[HttpGet]
		//[ResponseCache(Duration = 20, VaryByQueryKeys = new string[]{ "refresh", "new"})]
		public async Task<ActionResult<List<SupplierDto>>> GetAll()
		{
			return FromValueServiceResult(await _supplierService.GetAll());
		}

		[HttpGet("{id}", Name = Routes.GetActionById)]
		public async Task<ActionResult<SupplierDto>> GetById(int id)
		{
			return FromValueServiceResult(await _supplierService.GetSupplier(id));
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] SupplierDto value)
		{
			if (!value.RepresentsNewEntity)
				return BadRequest();

			var result = await _supplierService.CreateOrUpdate(value);

			if (!result.IsOk())
				return FromServiceResult(result);

			return CreatedAtRoute(Routes.GetActionById, new { id = result.Value.Id }, result.Value);
		}

		[HttpPut]
		public async Task<IActionResult> Update([FromBody] SupplierDto value)
		{
			if (value.RepresentsNewEntity)
				return BadRequest();

			var result = await _supplierService.CreateOrUpdate(value);

			if (!result.IsOk())
				return FromServiceResult(result);

			return NoContent();
		}

		[HttpDelete("{id}")]
		[Authorize(Policy = CustomPolicies.OnlyUsersOlderThan)]
		public async Task<IActionResult> Delete(int id)
		{
			var result = await _supplierService.Delete(id);

			if (!result.IsOk())
				return FromServiceResult(result);

			return NoContent();
		}
	}
}
