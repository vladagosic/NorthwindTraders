using Northwind.Application.Interfaces;

namespace Northwind.Application.Helpers
{
	public static class ServiceResultExtensions
	{
		public static ServiceResult<TValue> ToOkServiceResult<TValue>(this TValue value)
		{
			return ServiceResult.Ok(value);
		}

		public static bool IsOk(this IServiceResult result)
		{
			return result.Status == ServiceCallStatus.Ok;
		}
	}
}
