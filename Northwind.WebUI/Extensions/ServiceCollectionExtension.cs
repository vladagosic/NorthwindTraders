using MediatR;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Northwind.Application.Infrastructure;
using Northwind.Application.Interfaces;
using Northwind.Application.Products.Queries.GetProduct;
using Northwind.Application.Suppliers.Services;
using Northwind.Common;
using Northwind.Persistence.Infrastructure;
using Northwind.WebUI.Security;
using System.Reflection;
using System.Text;

namespace Northwind.WebUI.Extensions
{
	public static class ServiceCollectionExtension
	{
		public static IServiceCollection ConfigureMediator(this IServiceCollection services)
		{
			// Add MediatR
			services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));
			services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPerformanceBehaviour<,>));
			services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
			services.AddMediatR(typeof(GetProductQueryHandler).GetTypeInfo().Assembly);

			return services;
		}

		public static IServiceCollection ConfigureDataServices(this IServiceCollection services)
		{
			// Add repository
			services.AddScoped(typeof(IAsyncRepository<>), typeof(EfAsyncRepository<>));

			// Add services
			services.AddScoped<ISupplierService, SupplierService>();

			return services;
		}

		public static IServiceCollection ConfigureSecurity(this IServiceCollection services, string securityKey, int minimumAge)
		{
			// Add authentication JWT options settings.
			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(options =>
				{
					options.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuer = true,
						ValidateAudience = true,
						ValidateLifetime = true,
						ValidateIssuerSigningKey = true,
						ValidIssuer = "northwind.com",
						ValidAudience = "northwind.com",
						IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey))
					};
				});

			// Add custom authorization claim based policy.
			services.AddAuthorization(options =>
			{
				options.AddPolicy(
					CustomPolicies.OnlyUsersOlderThan,
					policy => policy
						.RequireClaim(CustomClaimTypes.DateOfBirth)
						.AddRequirements(new OlderThanRequirement(minimumAge)));
			});

			// Register Older Than authorization handler.
			services.AddSingleton<IAuthorizationHandler, OlderThanAuthorizationHandler>();

			return services;
		}
	}
}
