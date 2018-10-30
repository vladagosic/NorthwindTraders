using Northwind.Application.Suppliers.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Application.Interfaces
{
	public interface ISupplierService
	{
		Task<ServiceResult<SupplierDto>> GetSupplier(int id);
		Task<ServiceResult<List<SupplierDto>>> GetAll();
		Task<ServiceResult<SupplierDto>> CreateOrUpdate(SupplierDto supplierDto);
		Task<ServiceResult> Delete(int id);
	}
}
