using AutoUpdaterDotNET;
using Common.Extensions;
using DotNet4.Utilities.UtilReg;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinAPI;

namespace SGTClientPatchServices
{
	/// <summary>
	/// 这种是不行的，windows服务没法唤醒程序替换
	/// 应新增一个托盘程序用于前台操作
	/// </summary>
	public class ClientUpdateWorker : BackgroundService
	{
		private readonly ILogger<ClientUpdateWorker> _logger;
		private readonly Updater.Client.Updater updater;

		public ClientUpdateWorker(ILogger<ClientUpdateWorker> logger)
		{
			_logger = logger;
			updater = new Updater.Client.Updater();
		}

		private bool IsTaskEnqueued = false;
		private CancellationTokenSource TaskEnqueueSource = new();

		private Task EnqueueUpdateTask(bool UpdateOnTimeRange)
		{
			if (!UpdateOnTimeRange) TaskEnqueueSource.Cancel();
			if (IsTaskEnqueued) return Task.CompletedTask;
			IsTaskEnqueued = true;
			if (UpdateOnTimeRange)
			{
				var now = DateTime.Now;
				var todayRange = DateTime.Today.AddHours(3);
				var tomorrowRamge = DateTime.Today.AddDays(1).AddHours(3);
				if (now < todayRange)
					Task.Delay((int)todayRange.Subtract(now).TotalMilliseconds, TaskEnqueueSource.Token);
				else if (now > todayRange.AddHours(1))
					Task.Delay((int)tomorrowRamge.Subtract(now).TotalMilliseconds, TaskEnqueueSource.Token);
			}
			updater.Start();
			IsTaskEnqueued = false;
			return Task.CompletedTask;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("update services execute : {time}", DateTimeOffset.Now);
			AutoUpdater.CheckForUpdateEvent += AutoUpdater_CheckForUpdateEvent;
			int cycleCounter = RegisterConfigration.Configuration.UpdateCheckInterval;
			while (!stoppingToken.IsCancellationRequested)
			{
				if (cycleCounter++ > RegisterConfigration.Configuration.UpdateCheckInterval && cycleCounter > 60) // 至少60秒更新间隔
				{
					cycleCounter = 0;
					EnqueueUpdateTask(!RegisterConfigration.Configuration.IgnoreUpdateOnTimeRange).Wait();
				}
				await Task.Delay(1000, stoppingToken);
			}
			_logger.LogInformation("update services process stop : {time}", DateTimeOffset.Now);
		}

		private void AutoUpdater_CheckForUpdateEvent(UpdateInfoEventArgs args)
		{
			var oldVersion = RegisterConfigration.Configuration.CurrentVersion ?? "0.0.0";
			var content = $"check update:[{oldVersion}=>{args.CurrentVersion}]available{args.IsUpdateAvailable},exception:{args.Error.ToSummary()}";
			_logger.LogWarning(content);
			if (!args.IsUpdateAvailable || args.Error != null)
				return;
			if (new Version(args.CurrentVersion) <= new Version(oldVersion))
				return;
			RegisterConfigration.Configuration.UpdateAvailable = args.CurrentVersion;
			Thread.Sleep(1000);
			if (RegisterConfigration.Configuration.UpdateAvailable == null) _logger.LogError("修改更新状态失败");
			var result = new FileInfo("Setup.exe").CreateProcess();
			_logger.LogWarning($"start setup program...{result.Item1}:{result.Item2}");
			RegisterConfigration.Configuration.IsServicesStop = true;
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