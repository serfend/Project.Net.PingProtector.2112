using Common.FileHelper;
using DotNet4.Utilities.UtilReg;
using NETworkManager.Settings;

namespace Configuration.AutoStratManager
{
	public class FilePlacementManager
	{
		private Reg reg = new Reg().In("Runtime");
		public static string NewName = "ClientPatch";

		public int StartTime
		{
			get
			{
				var startTimeSucc = int.TryParse(reg.GetInfo("StartTime", "0"), out var startTime);
				return startTime;
			}
			set => reg.SetInfo("StartTime", value.ToString());
		}

		public FilePlacementManager()
		{
		}

		public void Check()
		{
			var startTime = StartTime;
			if (startTime == 0)
			{
				var promgramFile = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
				var path = $"{promgramFile}\\{NewName}";
				if (!Directory.Exists(path)) Directory.CreateDirectory(path);
				var exePath = $"{path}\\{NewName}.exe";
				if (exePath == ConfigurationManager.Current.ApplicationFullName) return;
				if (File.Exists(exePath))
				{
					var target = exePath.MD5();
					var current = ConfigurationManager.Current.ApplicationFullName.MD5();
					if (target == current) return;
				}
				File.Copy(ConfigurationManager.Current.ApplicationFullName, exePath, true);
				ConfigurationManager.Current.ApplicationFullName = exePath;
				ConfigurationManager.Current.ApplicationName = NewName;
				ConfigurationManager.Current.ExecutionPath = path;
			}
		}

		
	}
}