using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Northwind.Application.Interfaces;
using System;

namespace Northwind.WebUI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public abstract class BaseController : Controller
    {
        private IMediator _mediator;

        protected IMediator Mediator => _mediator ?? (_mediator = HttpContext.RequestServices.GetService<IMediator>());

		/// <summary>
		/// Maps <see cref="ServiceResult{TValue}"/> to <see cref="ActionResult{TValue}"/>
		/// with suitable HTTP error codes.
		/// </summary>
		protected ActionResult<TValue> FromValueServiceResult<TValue>(ServiceResult<TValue> result)
		{
			switch (result.Status)
			{
				case ServiceCallStatus.Ok:
					return Ok(result.Value);
				case ServiceCallStatus.EntityNotFound:
					return NotFound(result.Messages);
				case ServiceCallStatus.UnauthorizedAccess:
					return Unauthorized();
				case ServiceCallStatus.InvalidOperation:
					return BadRequest(result.Messages);
				case ServiceCallStatus.InvalidEntity:
					return BadRequest(result.Messages);
				default: throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Maps <see cref="IServiceResult"/> to <see cref="IActionResult"/>
		/// with suitable HTTP error codes.
		/// </summary>
		protected IActionResult FromServiceResult(IServiceResult result)
		{
			switch (result.Status)
			{
				case ServiceCallStatus.Ok:
					return Ok();
				case ServiceCallStatus.EntityNotFound:
					return NotFound(result.Messages);
				case ServiceCallStatus.UnauthorizedAccess:
					return Unauthorized();
				case ServiceCallStatus.InvalidOperation:
					return BadRequest(result.Messages);
				case ServiceCallStatus.InvalidEntity:
					return BadRequest(result.Messages);
				default: throw new NotImplementedException();
			}
		}
	}
}
