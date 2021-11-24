using Microsoft.Win32;
using NETworkManager.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration.AutoStratManager
{
	public class FunctionByReg : BaseAutoStartManager
	{
		// Key where the autorun entries for the current user are stored
		private const string RunKeyCurrentUser = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

		public override bool IsEnabled()
		{
			var registryKey = Registry.CurrentUser.OpenSubKey(RunKeyCurrentUser);
			return registryKey?.GetValue(ConfigurationManager.Current.ApplicationName) != null;
		}

		public override Task EnableAsync()
		{
			return Task.Run(Enable);
		}

		public override void Enable()
		{
			if (IsEnabled()) return;
			var registryKey = Registry.CurrentUser.OpenSubKey(RunKeyCurrentUser, true);

			var command = $"{ConfigurationManager.Current.ApplicationFullName}";

			if (registryKey == null)
				return; // LOG

			registryKey.SetValue(ConfigurationManager.Current.ApplicationName, command);
			registryKey.Close();
		}

		public override Task DisableAsync()
		{
			return Task.Run(() => Disable());
		}

		public override void Disable()
		{
			if (!IsEnabled()) return;

			var registryKey = Registry.CurrentUser.OpenSubKey(RunKeyCurrentUser, true);

			if (registryKey == null)
				return; // LOG

			registryKey.DeleteValue(ConfigurationManager.Current.ApplicationName);
			registryKey.Close();
		}
	}
}