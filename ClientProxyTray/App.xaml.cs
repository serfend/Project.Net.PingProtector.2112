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
			var buffer = new byte[1024];
			var clientPipe = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
			//异步链接服务端
			clientPipe.ConnectAsync(1000).ContinueWith(x =>
			{
				if (x.Exception == null)
				{
					cts.Send(new SendOrPostCallback((d) =>
					{
						Application.Current.Shutdown();
						return;
					}), null);
					return; // 连接成功，说明多开
				}
			}).Wait();
			NewConnection(); // 建立监听
			new FunctionByReg().EnableAsync().Wait();
			base.OnStartup(e);
		}
		private void NewConnection()
		{
			var serverPipe = new NamedPipeServerStream(pipeName, PipeDirection.InOut, 10, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
			serverPipe.WaitForConnectionAsync()
				.ContinueWith(x => NewConnection()); // 连接成功后开始下一次的监听
		}
	}
}
