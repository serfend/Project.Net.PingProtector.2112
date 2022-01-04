using AutoUpdaterDotNET;
using Newtonsoft.Json;

namespace Updater
{
	public static class Updater
	{
		public static event EventHandler<UpdateInfoEventArgs>? RequiredExitProgram;
		public static void Start()
		{
			AutoUpdater.InstalledVersion = new Version(1, 0, 0);
			AutoUpdater.RunUpdateAsAdmin = true;
			AutoUpdater.RemindLaterAt = 7;
			AutoUpdater.RemindLaterTimeSpan = RemindLaterFormat.Days;
			AutoUpdater.InstallationPath = "D:\\bin";
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
						MinimumVersion = mandatory?.minVersion
					},
					CheckSum = checkSum?.value == null ? null : new CheckSum
					{
						Value = checkSum?.value,
						HashingAlgorithm = checkSum?.hashingAlgorithm ?? "SHA1"
					},
					InstallerArgs = config?.installerArgs ?? string.Empty,
				};
			};
			AutoUpdater.Start("http://localhost:38080/1.json");
		}
	}
}