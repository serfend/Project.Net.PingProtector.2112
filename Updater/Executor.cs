using AutoUpdaterDotNET;
using Newtonsoft.Json;
using UpdaterClient.Events;

namespace Updater.Client
{
	public class Updater
	{
		public Updater() : this("1.3.68")
		{
		}

		public Updater(string CurrentVersion)
		{
			this.CurrentVersion = CurrentVersion;
		}

		public event EventHandler<UpdateInfoEventArgs>? RequiredExitProgram;

		public event EventHandler<UpdateInfoEventArgs>? RequiredResetProgram;

		public string? InstallationPath { get; set; }
		public string? ServerPath => RegisterConfigration.Configuration.ServerHost;

		public string CurrentVersion { get; }

		public void StartDownload(UpdateInfoEventArgs e)
		{
			if (AutoUpdater.DownloadUpdate(e))
				RequiredExitProgram?.Invoke(null, e);
			else
				RequiredResetProgram?.Invoke(null, e);
		}

		public void Start()
		{
			if (ServerPath == null)
			{
				OnUpdateServerNotSet?.Invoke(this, new UpdateServerNotSetEventArgs());
				return; // 未指定服务器，停止本次更新
			}

			AutoUpdater.RunUpdateAsAdmin = true;
			AutoUpdater.RemindLaterAt = 7;
			AutoUpdater.RemindLaterTimeSpan = RemindLaterFormat.Days;
			if (InstallationPath != null) AutoUpdater.InstallationPath = InstallationPath;
			AutoUpdater.InstalledVersion = new Version(CurrentVersion);
			AutoUpdater.ApplicationExitEvent += () =>
			{
				Console.WriteLine("ApplicationExitEvent");
			};
			AutoUpdater.CheckForUpdateEvent += (e) =>
			{
				Console.WriteLine("CheckForUpdateEvent");
				if (e.Error != null) return;
				if (!e.IsUpdateAvailable) return;
			};
			AutoUpdater.ParseUpdateInfoEvent += (e) =>
			{
				dynamic? config = JsonConvert.DeserializeObject(e.RemoteData);
				if (config is null || config?.url == null) return;
				Console.WriteLine("ParseUpdateInfoEvent");
				dynamic? mandatory = config?.mandatory;
				dynamic? checkSum = config?.checkSum;
				e.UpdateInfo = new UpdateInfoEventArgs()
				{
					CurrentVersion = config?.version ?? AutoUpdater.InstalledVersion,
					ChangelogURL = config?.changelog ?? string.Empty,
					DownloadURL = config?.url,
					Mandatory = mandatory == null ? null : new Mandatory
					{
						Value = mandatory?.value,
						UpdateMode = mandatory?.mode,
						MinimumVersion = mandatory?.minVersion,
					},
					CheckSum = checkSum?.value == null ? null : new CheckSum
					{
						Value = checkSum?.value,
						HashingAlgorithm = checkSum?.hashingAlgorithm ?? "SHA1"
					},
					InstallerArgs = config?.installerArgs ?? string.Empty,
				};
			};

			AutoUpdater.Start(ServerPath);
		}

		public event EventHandler<UpdateServerNotSetEventArgs>? OnUpdateServerNotSet;
	}
}