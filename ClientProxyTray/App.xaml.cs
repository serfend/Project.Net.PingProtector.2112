using Common.Extensions;
using Configuration.AutoStratManager;
using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ClientProxyTray
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public App()
		{
		}
		private string pipeName = $".\\Private$\\{Process.GetCurrentProcess().ProcessName}";
		protected override void OnStartup(StartupEventArgs e)
		{
			var cts = System.Windows.Forms.WindowsFormsSynchronizationContext.Current;
			new ProcessInstance(pipeName).CheckInstace(() =>
			{
				cts.Send(new SendOrPostCallback((d) =>
				{
					Application.Current.Shutdown();
					return;
				}), null);
				return; // 连接成功，说明多开
			});
			new FunctionByReg().EnableAsync().Wait();
			base.OnStartup(e);
		}
	}
}
