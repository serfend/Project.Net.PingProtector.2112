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
		private string pipeName = $"Inst_{Process.GetCurrentProcess().ProcessName}";
		protected override void OnStartup(StartupEventArgs e)
		{
			if (new ProcessInstance(pipeName).CheckInstaceByMutex()) {
				Environment.Exit(0);
				return;
			}
			new FunctionByReg().EnableAsync().Wait();
			base.OnStartup(e);
		}
	}
}
