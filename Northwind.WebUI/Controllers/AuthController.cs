using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Northwind.WebUI.Security;
using Northwind.WebUI.Security.Models;
using System;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Northwind.WebUI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : Controller
	{
		private readonly IConfiguration _configuration;

		public AuthController(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		[AllowAnonymous]
		[HttpPost("token")]
		public IActionResult RequestToken([FromBody] TokenRequest request)
		{
			// Sample implementation only for this simple demo.
			// We hardcode all the values related to the user. 
			// In reality, the UserService (which we will add later)
			// should be used here.
			if (request.Username != _configuration["User:Username"] || request.Password != _configuration["User:Password"])
				return BadRequest("Invalid username or password.");

			var claims = new[]
			{
                // Add well-known claim types.
                new Claim(ClaimTypes.Name, request.Username),
				new Claim(ClaimTypes.Role, UserRoles.User),

                // Add our custom string and non-string claim types.
				new Claim(CustomClaimTypes.DateOfBirth,
						  DateTime.Parse(_configuration["User:DateOfBirth"]).ToString("u", CultureInfo.InvariantCulture),
						  ClaimValueTypes.DateTime)
			};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecurityKey"]));
			var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: "northwind.com",
				audience: "northwind.com",
				claims: claims,
				expires: DateTime.Now.AddMinutes(30),
				signingCredentials: credentials);

			return Ok(new
			{
				token = new JwtSecurityTokenHandler().WriteToken(token)
			});
		}
	}
}
