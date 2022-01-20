using DotNet4.Utilities.UtilReg;
using System.Diagnostics;
using System.Windows.Forms;
using WinAPI;

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

		private void NewProcess()
		{
			if (DateTime.Now.Subtract(LastNewProcess).TotalSeconds < 15) return; // 至少15秒后再启动
			var path = Path.Join(AppDomain.CurrentDomain.BaseDirectory, $"{Project.Core.Protector.Main.PackageName}.exe");
			var (result, status) = new FileInfo(path).CreateProcess();
			LastNewProcess = DateTime.Now;
			if (result != WTSapi32.CreateProcessResult.Success)
			{
				_logger.LogError($"进程启动失败:{result}:{status}");
			}
		}

		private DateTime LastNewProcess { get; set; }

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("services execute : {time}", DateTimeOffset.Now);
			while (!stoppingToken.IsCancellationRequested)
			{
				var signal = (RegisterConfigration.Configuration.IsRunning ? 1 : 0) + (TargetProcessDied ? 1 : 0);
				switch (signal)
				{
					case 0:
						break;

					case 2:
						_logger.LogInformation("start new process : {time}", DateTimeOffset.Now);
						NewProcess();
						break;

					default:
						break;
				}
				await Task.Delay(1000, stoppingToken);
			}
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
		private bool TargetProcessDied
		{
			get
			{
				var client_time = RegisterConfigration.Configuration.CurrentRunningInstanceActive;
				var services_time = DateTimeOffset.Now.ToUnixTimeMilliseconds();
				var isDied = (services_time - client_time) / 1e3 > 15;
				return isDied;
			}
		}

		public override Task StopAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("services stop : {time}", DateTimeOffset.Now);
			return base.StopAsync(stoppingToken);
		}
	}
}