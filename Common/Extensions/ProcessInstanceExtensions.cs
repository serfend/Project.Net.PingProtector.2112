using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Extensions
{
	public class ProcessInstance
	{
		public ProcessInstance(string name)
		{
			Name = name;
		}

		public string Name { get; set; }
	}
	public static class ProcessInstanceExtensions
	{
		/// <summary>
		/// 通过创建命名管道检查是否存在进程重复开启的情况
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="callback"></param>
		public static void CheckInstace(this ProcessInstance instance, Action callback)
		{
			//var buffer = new byte[1024];
			using (var clientPipe = new NamedPipeClientStream(".", instance.Name, PipeDirection.InOut, PipeOptions.Asynchronous))
				//异步链接服务端
				clientPipe
					.ConnectAsync(1000)
					.ContinueWith(x =>
				{
					if (x.Exception == null)
					{
						callback.Invoke();
					}
				}).Wait();
			NewConnection(instance.Name); // 建立监听
		}

		private static void NewConnection(string pipeName)
		{
			using (var serverPipe =
				new NamedPipeServerStream(pipeName, PipeDirection.InOut, 10, PipeTransmissionMode.Message, PipeOptions.Asynchronous))
				serverPipe
					.WaitForConnectionAsync()
					.ContinueWith(x => NewConnection(pipeName)); // 连接成功后开始下一次的监听
		}
	}
}
