using Microsoft.AspNetCore.Authorization;

namespace Northwind.WebUI.Security
{
	/// <summary>
	/// Older than authorization requirement specifies that
	/// for certain action, a certain <see cref="MinimumAge"/>
	/// is required from the user.
	/// </summary>
	public class OlderThanRequirement : IAuthorizationRequirement
	{
		public int MinimumAge { get; }

		public OlderThanRequirement(int minimumAge)
		{
			MinimumAge = minimumAge;
		}
	}
}
