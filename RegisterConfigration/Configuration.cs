using DotNet4.Utilities.UtilReg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegisterConfigration
{
	public static partial class Configuration
	{
		private static Reg processConfig = new Reg().In("ServicesWorker");

		/// <summary>
		/// 主进程是否在运行
		/// </summary>
		public static bool IsRunning
		{
			get => int.Parse(processConfig.GetInfo("IsRunning", "0") ?? "0") > 0;
			set => processConfig.SetInfo("IsRunning", value ? 1 : 0, RegValueKind.DWord);
		}

		private static readonly CancellationTokenSource globalToken = new();

		/// <summary>
		/// 服务是否停止
		/// </summary>
		public static bool IsServicesStop
		{
			get => int.Parse(processConfig.GetInfo("IsServicesStop", "0") ?? "0") > 0;
			set
			{
				processConfig.SetInfo("IsServicesStop", value ? 1 : 0, RegValueKind.DWord);
				if (value) globalToken.Cancel();
			}
		}

		/// <summary>
		/// 当前新版本的版本
		/// </summary>
		public static string? UpdateAvailable
		{
			get => processConfig.GetInfo("UpdateAvailable");
			set => processConfig.SetInfo("UpdateAvailable", value);
		}

		public static long CurrentRunningInstanceActive
		{
			get => long.Parse(processConfig.GetInfo("CurrentRunningInstanceActive", "0") ?? "0");
			set => processConfig.SetInfo("CurrentRunningInstanceActive", value, RegValueKind.QWord);
		}

		/// <summary>
		/// 可通联的服务器地址
		/// </summary>
		public static string? ServerHost
		{
			get => processConfig.GetInfo("ServerHost");
			set => processConfig.SetInfo("ServerHost", value);
		}

		/// <summary>
		/// 当前已安装的版本
		/// </summary>
		public static string? CurrentVersion
		{
			get => processConfig.GetInfo("CurrentVersion");
			set => processConfig.SetInfo("CurrentVersion", value);
		}

		/// <summary>
		/// 当前激活的实例
		/// </summary>
		public static string? CurrentRunningInstance
		{
			get => processConfig.GetInfo("CurrentRunningInstance");
			set => processConfig.SetInfo("CurrentRunningInstance", value);
		}

		public static CancellationToken GlobalToken => globalToken.Token;
	}

	public static partial class Configuration
	{
		private static int DefaultUpdateCheckInterval = 10000;
		private static int UpdateCheckTimes = 0;

		/// <summary>
		/// 更新时长
		/// </summary>
		public static int UpdateCheckInterval
		{
			get
			{
				if (UpdateCheckTimes++ > 10)
				{
					UpdateCheckTimes = 0;
					DefaultUpdateCheckInterval = int.Parse(processConfig.GetInfo("UpdateCheckInterval", "0") ?? "0");
				}
				return DefaultUpdateCheckInterval;
			}
			set => processConfig.SetInfo("UpdateCheckInterval", value, RegValueKind.DWord);
		}
	}
}