using MahApps.Metro.Controls;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Threading;
using System.Timers;
using System.Windows;

namespace ClientProxyTray
{
	public partial class MainWindow : MetroWindow
	{
		private readonly ILogger<MainWindow> _logger;
		private readonly Updater.Client.Updater updater = new();
		System.Timers.Timer timer = new ()
		{
			Enabled = true,
			Interval = 1000
		};
		public MainWindow()
		{
			
			_logger = new Logger<MainWindow>(new LoggerFactory());
			_logger.LogInformation("tray start");
			InitUpdateChecker();
		}
		private void InitUpdateChecker()
		{
			updater.OnUpdateServerNotSet += Updater_OnUpdateServerNotSet;
			updater.RequiredExitProgram += Updater_RequiredExitProgram;
			timer.Elapsed += Timer_Elapsed;
		}

		private void Updater_RequiredExitProgram(object? sender, AutoUpdaterDotNET.UpdateInfoEventArgs e)
		{
			_logger.LogWarning("tray Updater_RequiredExitProgram");
			Application.Current.Shutdown();
		}

		private void Updater_OnUpdateServerNotSet(object? sender, UpdaterClient.Events.UpdateServerNotSetEventArgs e)
		{
			_logger.LogInformation("tray Updater_OnUpdateServerNotSet");
		}
		private int updateCounter = 0;
		private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
		{
			if (updateCounter-- > 0) return;
			_logger.LogInformation("tray Update.Start");
			updateCounter = 10000;
			updater.Start();
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			_logger.LogInformation("tray closing");
			SfTray.Dispose();
			base.OnClosing(e);
		}
		private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
		{
			var toggleSwitch = sender as ToggleSwitch;
			if (toggleSwitch == null) return;
			if (toggleSwitch.IsOn == true)
			{
				progress.IsActive = true;
				progress.Visibility = Visibility.Visible;
				return;
			}
			progress.IsActive = false;
			progress.Visibility = Visibility.Collapsed;
		}

		private void FlipViewst_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			var flipview = ((FlipView)sender);
			switch (flipview.SelectedIndex)
			{
			}
		}

	}
}
