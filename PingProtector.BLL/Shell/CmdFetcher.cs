using DevServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingProtector.BLL.Shell
{
	public class CmdFetcher
	{
		public event EventHandler<NewCmdEventArgs> OnNewCmdReceived;

		public readonly List<string> host;
		public readonly string path;

		public CmdFetcher(List<string> host, string path)
		{
			this.host = host;
			this.path = path;
			Task.Run(() =>
			{
				Task.Delay(5000);
				foreach (var ip in host)
				{
					using (var r = new Reporter())
					{
						var result = r.Report(ip, path, null, "get");
						OnNewCmdReceived?.Invoke(this, new NewCmdEventArgs()
						{
							Success = result.IsSuccessStatusCode,
							Message = result.Content.ReadAsStringAsync().Result
						});
					}
				}
			});
		}
	}

	public class NewCmdEventArgs : EventArgs
	{
		public bool Success { get; set; }
		public string Message { get; set; }
	}
}