using System.Diagnostics;

namespace EventLogHandler
{
	public static class GlobalEventLog
	{
		public const string LogModuleName = "DefaultModule";
		public const string LogDefaultSourceName = "ClientPatch";
		public static Dictionary<string, EventLog> LogDict = new();

		public static EventLog GetLog(string name = LogModuleName)
		{
			if (!LogDict.ContainsKey(name))
			{
				if (!EventLog.SourceExists(LogDefaultSourceName))
					EventLog.CreateEventSource(LogDefaultSourceName, name);
				LogDict[name] = new EventLog(name)
				{
					Source = LogDefaultSourceName,
				};
			}
			return LogDict[name];
		}

		public static EventLog DefaultLogger => GetLog();
	}
}