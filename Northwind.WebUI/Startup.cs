using FluentValidation.AspNetCore;
using MediatR;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Northwind.Application.Customers.Commands.CreateCustomer;
using Northwind.Application.Infrastructure;
using Northwind.Application.Interfaces;
using Northwind.Application.Products.Queries.GetProduct;
using Northwind.Application.Suppliers.Services;
using Northwind.Persistence;
using Northwind.Persistence.Infrastructure;
using Northwind.Persistence.Interfaces;
using Northwind.WebUI.Filters;
using Northwind.WebUI.Security;
using NSwag.AspNetCore;
using System.Reflection;
using System.Text;

namespace Northwind.WebUI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            // Add MediatR
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPerformanceBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
            services.AddMediatR(typeof(GetProductQueryHandler).GetTypeInfo().Assembly);

			// Add repository
			services.AddScoped(typeof(IAsyncRepository<>), typeof(EfAsyncRepository<>));

			// Add services
			services.AddScoped<ISupplierService, SupplierService>();

			// Add DbContext using SQL Server Provider
			services.AddDbContext<NorthwindDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("NorthwindDatabase")));


            services
                .AddMvc(options => options.Filters.Add(typeof(CustomExceptionFilterAttribute)))
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<CreateCustomerCommandValidator>());

            // Customise default API behavour
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

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
						IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecurityKey"]))
					};
				});

			// Add custom authorization claim based policy.
			services.AddAuthorization(options =>
			{
				options.AddPolicy(
					CustomPolicies.OnlyUsersOlderThan,
					policy => policy
						.RequireClaim(CustomClaimTypes.DateOfBirth)
						.AddRequirements(new OlderThanRequirement(Configuration.GetValue<int>("MinimumAge"))));
			});

			// Register Older Than authorization handler.
			services.AddSingleton<IAuthorizationHandler, OlderThanAuthorizationHandler>();

			// Add versioning API.
			services.AddApiVersioning(v =>
			{
				v.AssumeDefaultVersionWhenUnspecified = true;
				v.ApiVersionReader = new HeaderApiVersionReader("api-version");
			});
		}

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

			// Use authentication. Without it e.g. [Authorize] attribute
			// will not work.
			app.UseAuthentication();

			app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseSwaggerUi3(settings =>
            {
                settings.SwaggerUiRoute = "/api";
                settings.SwaggerRoute = "/api/specification.json";
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
                }
            });
        }
    }
}
