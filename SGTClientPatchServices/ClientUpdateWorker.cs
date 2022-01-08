using AutoUpdaterDotNET;
using Common.Extensions;
using DotNet4.Utilities.UtilReg;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
		}


		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("update services execute : {time}", DateTimeOffset.Now);
			AutoUpdater.CheckForUpdateEvent += AutoUpdater_CheckForUpdateEvent;
			int cycleCounter = 10000;
			while (!stoppingToken.IsCancellationRequested)
			{
				if (cycleCounter++ > 10000)
				{
					cycleCounter = 0;
					updater.Start();
				}
				await Task.Delay(1000, stoppingToken);
			}
			_logger.LogInformation("update services process stop : {time}", DateTimeOffset.Now);
		}

		private void AutoUpdater_CheckForUpdateEvent(UpdateInfoEventArgs args)
		{
			if (!args.IsUpdateAvailable || args.Error != null)
			{
				_logger.LogWarning($"check update:available{args.IsUpdateAvailable},exception:{args.Error.ToSummary()}");
				return;
			}
			_logger.LogWarning("start setup program...");
			RegisterConfigration.Configuration.UpdateAvailable = true;
			RegisterConfigration.Configuration.IsServicesStop = true;
			Process.Start("Setup.exe");
		}

		public override Task StartAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation($"current version:{updater.CurrentVersion},update services start : {DateTimeOffset.Now}");
			return base.StartAsync(stoppingToken);
		}
		public override Task StopAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("update services stop : {time}", DateTimeOffset.Now);
			return base.StopAsync(stoppingToken);
		}

	}
}
