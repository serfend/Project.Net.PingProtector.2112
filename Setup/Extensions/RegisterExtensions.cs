using DotNet4.Utilities.UtilReg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Setup.Extensions
{
	public static class RegisterExtensions
	{
		public static string GetServicesConfig(this ServiceRegister r, string ConfigName, string defaultValue)
		=> r.Reg.GetInfo(ConfigName, defaultValue);
		public static void SetServicesConfig(this ServiceRegister r, string ConfigName, object value, RegValueKind valueKind = RegValueKind.String) => r.Reg.SetInfo(ConfigName, value, valueKind);
		/// <summary>
		/// 移除服务，需要重启生效
		/// </summary>
		/// <param name="r"></param>
		public static void RemoveService(this ServiceRegister r) => r.Reg.Delete();
		/// <summary>
		/// 安装服务，立即生效
		/// </summary>
		/// <param name="r"></param>
		/// <param name="exePath"></param>
		/// <param name="brand"></param>
		/// <param name="startType"></param>
		/// <param name="description"></param>
		public static void InstallService(this ServiceRegister r, string exePath, string? brand = null, ServiceStartType startType = ServiceStartType.Auto, string? description = null)
		{
			r.SetServicesConfig("Description", description ?? "");
			r.SetServicesConfig("ImagePath", exePath);
			r.SetServicesConfig("DisplayName", brand ?? r.PackageName);
			r.SetServicesConfig("ObjectName", "LocalSystem");
			r.SetServicesConfig("ErrorControl", 1, RegValueKind.DWord);
			r.SetServicesConfig("DelayedAutostart", startType == ServiceStartType.AutoWithDelay ? 1 : 0, RegValueKind.DWord);
			r.SetServicesConfig("Start", (int)startType, RegValueKind.DWord);
			r.SetServicesConfig("Type", 16, RegValueKind.DWord);
		}
		public static void InitNewService(this ServiceRegister r) => throw new NotImplementedException();
		public enum ServiceStartType
		{
			Auto = 2,
			AutoWithDelay = 1,
			Manual = 3,
			Ban = 4
		}
	}
	public class ServiceRegister
	{
		public string RegPath { get; set; }
		public Reg Reg { get; set; }
		public ServiceRegister(string packageName)
		{
			PackageName = packageName;
		}
		private string? packageName;
		public string PackageName
		{
			get => packageName ?? string.Empty;
			set
			{
				packageName = value;
				RegPath = $"SYSTEM\\ControlSet001\\Services\\{PackageName}";
				Reg = new Reg(RegPath, RegDomain.LocalMachine);
			}
		}
	}
}
