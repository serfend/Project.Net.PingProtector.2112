using AutoUpdaterDotNET;
using Common.CmdShellHelper;
using Common.PowershellHelper;
using DotNet4.Utilities.UtilReg;
using Microsoft.Extensions.Logging;
using PermissionManager;
using PingProtector.BLL.Shell;
using Setup.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Setup.Extensions.RegisterExtensions;
using static Setup.Extensions.StartUpExtensions;

namespace Setup
{
	public class SetupExecutor
	{
		private ILogger<SetupExecutor> logger;

		private static bool waitingForUpdate = false;
		private static Updater.Client.Updater updater = new();

		private FileMover migrator;

		public SetupExecutor(FileMover migrator, ILogger<SetupExecutor> logger)
		{
			this.migrator = migrator;
			this.logger = logger;
		}

		private const string UpdateSuccessFlag = "#UpdateSuccessFlag#";

		/// <summary>
		/// 检查是新版本还是更新成功标识
		/// </summary>
		/// <param name="raw"></param>
		/// <returns></returns>
		private (bool, DateTime, Version?) ExtractUpdateSuccessFlag(string? raw)
		{
			if (raw == null) return (false, DateTime.MinValue, null);
			var result = raw.StartsWith(UpdateSuccessFlag);
			if (!result) return (false, DateTime.MinValue, new Version(raw));
			var d = raw.Substring(UpdateSuccessFlag.Length);
			_ = DateTime.TryParse(d, out var s);
			return (true, s, null);
		}

		public void Run(string[] args)
		{
			RegisterConfigration.Configuration.IsRunning = false;
			var oldVersion = RegisterConfigration.Configuration.CurrentVersion;
			var isNewVersion = ExtractUpdateSuccessFlag(RegisterConfigration.Configuration.UpdateAvailable);
			while (!isNewVersion.Item1 && isNewVersion.Item3 != null)
			{
				logger.LogWarning($"发现新版本:{isNewVersion.Item3},开始下载和安装");
				AutoUpdater.CheckForUpdateEvent += (e) =>
				{
					RegisterConfigration.Configuration.CurrentVersion = isNewVersion.Item3.ToString();
					RegisterConfigration.Configuration.UpdateAvailable = $"{UpdateSuccessFlag}{DateTime.Now}";
					updater.StartDownload(e);
					waitingForUpdate = false;
				};
				updater.OnUpdateServerNotSet += (s, e) =>
				{
					logger.LogWarning($"未设置服务器地址，取消下载");
					Thread.Sleep(5000);
					Application.Exit();
				};
				updater.RequiredResetProgram += (s, e) =>
				{
					logger.LogWarning($"下载失败，等待下次重试");
					RegisterConfigration.Configuration.CurrentVersion = oldVersion;
					ResetServicesAndConfig();
					Application.Exit();
				};
				waitingForUpdate = true;
				updater.Start();
				Waiting();
				return;
			}
			// 为已安装完成，则正常启动服务
			while (isNewVersion.Item1)
			{
				ResetServicesAndConfig();
				return;
			}

			// 否则则正常安装
			var vNew = new Version(updater.CurrentVersion);
			var vOld = new Version(RegisterConfigration.Configuration.CurrentVersion ?? "1.0.0");
			logger.LogInformation($"version update:{vOld}->{vNew}");
			if (vNew <= vOld)
			{
				logger.LogWarning($"当前已安装的版本是更加新的版本({vOld})，取消安装({vNew})");
				Thread.Sleep(5000);
				return;
			}
			Install(args);
			RegisterConfigration.Configuration.CurrentVersion = vNew.ToString();
		}

		private void ResetServicesAndConfig()
		{
			new CmdExecutor().CmdRun("start services", "sc start ClientPatch");
			RegisterConfigration.Configuration.UpdateAvailable = null;
			Thread.Sleep(5000);
		}

		private void Waiting()
		{
			int timeout = 120;
			while (waitingForUpdate && timeout-- > 0)
			{
				Thread.Sleep(1000);
			}
			if (waitingForUpdate)
			{
				logger.LogError($"等待服务器下载超时，请联系管理员");
				Thread.Sleep(10000);
				Application.Exit();
			}
		}

		/// <summary>
		/// 当使用UAC时将导致进程提权启动报错
		/// 需要通过禁用EnableLua解决
		/// </summary>
		private void RegisterUACDisable()
		{
			var Key_EnableLua = "EnableLUA";
			var enableLua = new Reg(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System");
			if (int.Parse(enableLua.GetInfo(Key_EnableLua) ?? "0") == 1)
			{
				enableLua.SetInfo(Key_EnableLua, 0, RegValueKind.DWord);
			}
		}

		private void Install(string[] args)
		{
			var startUpType = args.GetStartUpType();
			var targetPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
			logger.LogWarning($"即将安装{Program.brand}。\n以[{Environment.UserDomainName}/{Environment.UserName}][{string.Join('|', WindowsIdentity.GetCurrent().GetClaims())}]权限运行");
			Thread.Sleep(5000);
			logger.LogWarning($"开始迁移文件");
			migrator.Init(Program.packageName, null, targetPath);
			migrator.OnFileMigrate += (s, e) =>
			{
				if (e.FileStatus.HasFlag(WinAPI.FileHandlerExtensions.FileStatus.IsOccupy))
				{
				}
			};
			migrator.Migrate();

			logger.LogWarning($"开始安装服务");
			RegisterServices(startUpType);
			RegisterUACDisable();
			logger.LogWarning("完成安装");

			// 临时启动
			Process.Start(Path.Combine(migrator.DstPath, $"ClientPatch.exe"));
			Process.Start(Path.Combine(migrator.DstPath, $"{nameof(ClientProxyTray)}.exe"));
			if (MessageBox.Show(null, "安装完成，需要重启", "完成", MessageBoxButtons.OKCancel) == DialogResult.OK)
				PowerShellHelper.ExecuteCommand($"shutdown -r -t 0");
		}

		private void RegisterServices(StartUpType startUpType)
		{
			var exePath = Path.Combine(migrator.DstPath, $"SGTClientPatchServices.exe");
			var r = new ServiceRegister(Program.packageName);
			var count = r.Reg?.InnerKey?.ValueCount ?? 0;
			logger.LogWarning($"检查服务字段:{count}总数");
			if (count > 0)
			{
				logger.LogWarning($"移除老版服务");
				r.RemoveService();
			}
			logger.LogWarning($"安装新版服务");
			r.InstallService(exePath, Program.brand, ServiceStartType.Auto, Program.description);
		}
	}
}