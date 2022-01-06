using DotNet4.Utilities.UtilReg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGTClientPatchServices
{
	public static class Configuration
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
		private static readonly CancellationTokenSource? globalToken = new();
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
		public static CancellationToken GlobalToken => globalToken.Token;
	}
}
