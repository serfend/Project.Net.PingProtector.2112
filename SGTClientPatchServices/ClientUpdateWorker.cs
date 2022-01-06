using DotNet4.Utilities.UtilReg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGTClientPatchServices
{
	public class ClientUpdateWorker : BackgroundService
	{
		private readonly ILogger<ClientUpdateWorker> _logger;
		private readonly Updater.Client.Updater updater;
		public ClientUpdateWorker(ILogger<ClientUpdateWorker> logger)
		{
			_logger = logger;
			updater = new Updater.Client.Updater();
			updater.RequiredExitProgram += Updater_RequiredExitProgram;
		}

		private void Updater_RequiredExitProgram(object? sender, AutoUpdaterDotNET.UpdateInfoEventArgs e)
		{
			Configuration.IsRunning = false;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("update services execute : {time}", DateTimeOffset.Now);
			int testTime = 10;
			while (!stoppingToken.IsCancellationRequested)
			{
				if (testTime-- == 0) Configuration.IsServicesStop = true;
				await Task.Delay(1000, stoppingToken);
			}
			_logger.LogInformation("update services process stop : {time}", DateTimeOffset.Now);
		}
		public override Task StartAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("update services start : {time}", DateTimeOffset.Now);
			return base.StartAsync(stoppingToken);
		}
		public override Task StopAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("update services stop : {time}", DateTimeOffset.Now);
			return base.StopAsync(stoppingToken);
		}

	}
}
