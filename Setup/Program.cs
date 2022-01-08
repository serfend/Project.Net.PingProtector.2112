

using AutoUpdaterDotNET;
using Common.PowershellHelper;
using DotNet4.Utilities.UtilReg;
using PingProtector.BLL.Shell;
using Serilog;
using Setup;
using Setup.Extensions;
using System.Diagnostics;
using System.Security.Principal;
using System.Windows.Forms;
using static Setup.Extensions.RegisterExtensions;

namespace Setup
{
	public static class Program
	{

		public static string brand = Project.Core.Protector.Main.BrandName;
		public static string packageName = Project.Core.Protector.Main.PackageName;
		public static string servicesName = nameof(SGTClientPatchServices);
		public static string description = Project.Core.Protector.Main.Description;
		private static FileMover? migrator = null;
		private enum StartUpType
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
		private static StartUpType GetStartUpType(string[] args)
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
		private static List<string> GetClaims()
		{
			var user_claims = new WindowsPrincipal(WindowsIdentity.GetCurrent());
			var claims = new List<string>();
			foreach (var i in Enum.GetValues(typeof(WindowsBuiltInRole)))
				if (user_claims.IsInRole((WindowsBuiltInRole)i)) claims.Add(i.ToString());
			return claims;
		}
		private static bool waitingForUpdate = false;
		private static Updater.Client.Updater updater = new();
		public static void Main(string[] args)
		{
			LogInitializer.Init();
			var hasNew = RegisterConfigration.Configuration.UpdateAvailable;
			while (hasNew)
			{
				Log.Warning($"发现新版本");
				RegisterConfigration.Configuration.UpdateAvailable = false;
				AutoUpdater.CheckForUpdateEvent += (e) =>
				{
					updater.StartDownload(e);
					waitingForUpdate = false;
				};
				updater.OnUpdateServerNotSet += (s, e) =>
				{
					Log.Warning($"未设置服务器地址，取消下载");
					Application.Exit();
				};
				updater.Start();
				break;
			}
			Waiting();
			var vNew = new Version(updater.CurrentVersion);
			var vOld = new Version(RegisterConfigration.Configuration.CurrentVersion ?? "1.0.0");
			if (vNew <= vOld)
			{
				Log.Warning($"当前已安装的版本是更加新的版本({vOld})，取消安装({vNew})");
				return;
			}
			RegisterConfigration.Configuration.CurrentVersion = vNew.ToString();
			Install(args);
		}
		private static void Waiting()
		{
			int timeout = 120;
			while (waitingForUpdate && timeout-- > 0)
			{
				Thread.Sleep(1000);
			}
			if (waitingForUpdate)
			{
				Log.Error($"等待服务器下载超时，请联系管理员");
				Thread.Sleep(10000);
				Application.Exit();
			}
		}
		private static void Install(string[] args)
		{

			var startUpType = GetStartUpType(args);
			var targetPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
			Log.Warning($"即将安装{brand}。\n以[{Environment.UserDomainName}/{Environment.UserName}][{string.Join('|', GetClaims())}]权限运行");
			Thread.Sleep(5000);
			Log.Warning($"开始迁移文件");
			migrator = new FileMover(packageName, null, targetPath);
			migrator.OnFileMigrate += (s, e) =>
			{
				if (e.FileStatus.HasFlag(WinAPI.FileHandlerExtensions.FileStatus.IsOccupy))
				{

				}
			};
			migrator.Migrate();


			Log.Warning($"开始安装服务");
			RegisterServices(startUpType);
			Log.Warning("完成安装");

			// 临时启动
			var exePath = Path.Combine(migrator.DstPath, $"ClientPatch.exe");
			Process.Start(exePath);
			if (MessageBox.Show(null, "安装完成，需要重启", "完成", MessageBoxButtons.OKCancel) == DialogResult.OK)
				PowerShellHelper.ExecuteCommand($"shutdown -r -t 0");
		}
		private static void RegisterServices(StartUpType startUpType)
		{
			var exePath = Path.Combine(migrator.DstPath, $"SGTClientPatchServices.exe");
			var r = new ServiceRegister(packageName);
			var count = r.Reg.InnerKey.ValueCount;
			Log.Warning($"检查服务字段:{count}总数");
			if (count > 0)
			{
				Log.Warning($"移除老版服务");
				r.RemoveService();
			}
			Log.Warning($"安装新版服务");
			r.InstallService(exePath, brand, ServiceStartType.Auto, description);

		}
	}
}