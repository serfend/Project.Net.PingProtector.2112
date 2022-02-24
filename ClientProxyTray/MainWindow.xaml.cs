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

		private System.Timers.Timer timer = new()
		{
			Enabled = true,
			Interval = 1000
		};

		public MainWindow()
		{
			_logger = new Logger<MainWindow>(new LoggerFactory());
			_logger.LogInformation("tray start");
			timer.Elapsed += Timer_Elapsed;
		}

		private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
		{
			if (!RegisterConfigration.Configuration.IsRunning)
			{
				_logger.LogInformation("require close");
				Environment.Exit(0);
			}
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

		private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
		{
			this.Visibility = Visibility.Hidden;
		}
	}
}