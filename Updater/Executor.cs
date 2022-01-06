using AutoUpdaterDotNET;
using Newtonsoft.Json;

namespace Updater.Client
{
	public class Updater
	{
		public Updater():this("1.0.7") { }
		public Updater(string CurrentVersion)
		{
			this.CurrentVersion = CurrentVersion;
		}
		public event EventHandler<UpdateInfoEventArgs>? RequiredExitProgram;
		public string? InstallationPath { get; set; }
		public string? ServerPath { get; set; }
		public string CurrentVersion { get; }

		public void Start()
		{
			AutoUpdater.RunUpdateAsAdmin = true;
			AutoUpdater.RemindLaterAt = 7;
			AutoUpdater.RemindLaterTimeSpan = RemindLaterFormat.Days;
			AutoUpdater.InstallationPath = InstallationPath ?? "D:\\bin";
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
				if (AutoUpdater.DownloadUpdate(e))
				{
					RequiredExitProgram?.Invoke(null, e);
				};
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
			AutoUpdater.Start(ServerPath ?? "http://localhost:38080/1.json");
		}
	}
}