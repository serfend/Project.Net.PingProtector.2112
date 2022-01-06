using DotNet4.Utilities.UtilReg;
using System.Diagnostics;
using System.Windows.Forms;

namespace SGTClientPatchServices
{
	public class Worker : BackgroundService
	{
		private readonly ILogger<Worker> _logger;
		private Reg processConfig = new Reg().In("ServicesWorker");
		public Worker(ILogger<Worker> logger)
		{
			_logger = logger;
		}
		private Process NewProcess()
		{
			return new Process()
			{
				StartInfo = new ProcessStartInfo()
				{
					FileName = Path.Join(AppDomain.CurrentDomain.BaseDirectory, $"{Project.Core.Protector.Main.PackageName}.exe"),
					UseShellExecute = true,
				},

			};
		}
		private Process? process = null;
		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("services execute : {time}", DateTimeOffset.Now);
			while (!stoppingToken.IsCancellationRequested)
			{
				var signal = (Configuration.IsRunning ? 1 : 0) + (TargetProcessDied ? 1 : 0);
				switch (signal)
				{
					case 0:
						process?.Kill();
						break;
					case 2:
						_logger.LogInformation("start new process : {time}", DateTimeOffset.Now);
						process = NewProcess();
						process.Start();
						break;
				}
				await Task.Delay(1000, stoppingToken);
			}
			if (!(process?.HasExited ?? true)) process?.Kill();
			_logger.LogInformation("services process stop : {time}", DateTimeOffset.Now);
		}
		public override Task StartAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("services start : {time}", DateTimeOffset.Now);
			return base.StartAsync(stoppingToken);
		}
		/// <summary>
		/// 是否应拉起被守护的进程
		/// </summary>
		private bool TargetProcessDied => process?.HasExited ?? true;
		public override Task StopAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("services stop : {time}", DateTimeOffset.Now);
			if (!TargetProcessDied) process?.Kill();
			return base.StopAsync(stoppingToken);
		}

	}
}