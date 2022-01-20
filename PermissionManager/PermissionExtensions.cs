using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace PermissionManager
{
	public static class PermissionExtensions
	{
		public static List<WindowsBuiltInRole> GetClaims(this WindowsIdentity? current)
		{
			current ??= WindowsIdentity.GetCurrent();
			var user_claims = new WindowsPrincipal(current);
			var claims = new List<WindowsBuiltInRole>();
			foreach (var i in Enum.GetValues(typeof(WindowsBuiltInRole)))
				if (user_claims.IsInRole((WindowsBuiltInRole)i)) claims.Add((WindowsBuiltInRole)i);
			return claims;
		}
	}
}