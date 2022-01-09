using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Setup.Extensions
{
	public static class StartUpExtensions
	{

		public enum StartUpType
		{
			/// <summary>
			/// 首次安装应先检查是否有先前的服务，有的话则需要加入到runonce待下次重启时重置新服务
			/// </summary>
			Default = 0,
			/// <summary>
			/// 为第二次重启，可以重置新服务
			/// </summary>
			ToRegisterServices = 1
		}
		public static StartUpType GetStartUpType(this string[] args)
		{
			if (args.Length == 0) return StartUpType.Default;
			switch (args[0])
			{
				case "InstallService":
					return StartUpType.ToRegisterServices;
				default:
					return StartUpType.Default;
			}
		}
		public static List<string> GetClaims(this WindowsIdentity? current)
		{
			current ??= WindowsIdentity.GetCurrent();
			var user_claims = new WindowsPrincipal(current);
			var claims = new List<string>();
			foreach (var i in Enum.GetValues(typeof(WindowsBuiltInRole)))
				if (user_claims.IsInRole((WindowsBuiltInRole)i)) claims.Add(i?.ToString() ?? "unknown");
			return claims;
		}
	}
}
