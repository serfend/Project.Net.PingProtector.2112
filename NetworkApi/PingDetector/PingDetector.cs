using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Project.Core.Protector.BLL.Network.PingDetector
{
	public class PingDetector
	{
		private readonly PingOptions options;
		private readonly byte[] buffer = Encoding.ASCII.GetBytes("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");

		/// <summary>
		/// host to ping
		/// </summary>
		public string[] Host { get; set; }

		public bool Enabled { get => timer.Enabled; set => timer.Enabled = value; }

		/// <summary>
		/// if set , it would raise event while ping success
		/// </summary>
		public double CheckInterval
		{
			get => timer.Interval;
			set
			{
				timer.Interval = value;
				if (!timer.Enabled && value >= 0) timer.Start();
				else if (value <= 0) timer.Stop();
			}
		}

		public event EventHandler<PingSuccessEventArgs> OnPingReply;

		public PingDetector(PingOptions options = null, string[] host = null)
		{
			if (options == null) options = new PingOptions
			{
				// Use the default Ttl value which is 128,
				// but change the fragmentation behavior.
				DontFragment = true
			};
			this.options = options;
			Host = host;
			timer = new System.Timers.Timer()
			{
				Enabled = false,
			};
			timer.Elapsed += Timer_Elapsed; ;
		}

		private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			Check();
		}

		public System.Timers.Timer timer;

		public IEnumerable<PingReply> Check(string[] hosts = null, int timeout = 3000)
		{
			if (hosts == null) hosts = Host;
			Debug.WriteLine($"check ping:{hosts.Length}");
			var r = new List<Task<PingReply>>();
			foreach (var host in hosts)
			{
				var waiter = new AutoResetEvent(false);
				var task = new Task<PingReply>(() =>
				{
					using (var Ping = new Ping())
					{
						PingReply reply = null;
						try
						{
							reply = Ping.Send(host, timeout, buffer, options);
						}
						catch (Exception)
						{
						}
						if (reply?.Status == IPStatus.Success) OnPingReply?.Invoke(this, new PingSuccessEventArgs(reply));
						return reply;
					}
				});
				task.Start();
				r.Add(task);
			}
			var tasks = r.ToArray();
			Task.WaitAll(tasks);
			return tasks.Select(t => t.Result);
		}
	}
}