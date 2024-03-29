﻿using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinAPI;
using static WinAPI.WTSapi32;

namespace Project.Net.PingProtector._2006.Services
{
	public static class LogServices
	{
		public const string LogFile_Main = "main";
		public const string LogFile_Detector = "detector";
		private const string ConfigContent = "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTgiID8+CjxubG9nIHhtbG5zPSJodHRwOi8vd3d3Lm5sb2ctcHJvamVjdC5vcmcvc2NoZW1hcy9OTG9nLnhzZCIKICAgICAgeG1sbnM6eHNpPSJodHRwOi8vd3d3LnczLm9yZy8yMDAxL1hNTFNjaGVtYS1pbnN0YW5jZSI+CgoJPHRhcmdldHM+CgkJPHRhcmdldCB4c2k6dHlwZT0iRmlsZSIgbmFtZT0iZmlsZV9tYWluIiBmaWxlTmFtZT0iJHtiYXNlZGlyfS9sb2dzL2xvZy4ke2V2ZW50LXByb3BlcnRpZXM6ZmlsZW5hbWV9LiR7c2hvcnRkYXRlfS5sb2cgIgogICAgICAgICAgICBsYXlvdXQ9IiR7bG9uZ2RhdGV9ICR7dXBwZXJjYXNlOiR7bGV2ZWx9fSAke21lc3NhZ2V9IiAvPgoJCTx0YXJnZXQgbmFtZT0ibG9nY29uc29sZSIgeHNpOnR5cGU9IkNvbnNvbGUiIC8+Cgk8L3RhcmdldHM+CgoJPHJ1bGVzPgoJCTxsb2dnZXIgbmFtZT0iKiIgbWlubGV2ZWw9IkRlYnVnIiB3cml0ZVRvPSJmaWxlX21haW4iIC8+Cgk8L3J1bGVzPgo8L25sb2c+";
		public static Logger mainLogger = LogManager.GetCurrentClassLogger().WithProperty("filename", LogServices.LogFile_Main);
		public static int ShowMessageBox(this IntPtr hwnd, string message, string title, DialogStyle flag = 0)
		{
			var t = new
			{
				hwnd = hwnd.ToString(),
				message,
				title,
				flag = flag.ToString()
			};
			mainLogger.Info($"MessageBox:{JsonConvert.SerializeObject(t)}");
			return WTSapi32.ShowMessageBox(hwnd, message, title, flag);
		}
		public static void ErrorLog(string message)
		{
			try
			{
				mainLogger.Error(message);
			}
			catch (Exception) { }
		}
		public static void Init()
		{
			var currentPath = AppDomain.CurrentDomain.BaseDirectory; // https://zhuanlan.zhihu.com/p/104469966
			var targetPath = Path.Combine("..", currentPath, "logs");
			if (!Directory.Exists(targetPath)) Directory.CreateDirectory(targetPath);
			var config_file = $"{currentPath}/nlog.config";
			if (!File.Exists(config_file))
				File.WriteAllText(config_file, Encoding.UTF8.GetString(Convert.FromBase64String(ConfigContent)));
		}
	}
}