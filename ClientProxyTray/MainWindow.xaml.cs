using MahApps.Metro.Controls;
using System.ComponentModel;
using System.Windows;

namespace ClientProxyTray
{
	public partial class MainWindow : MetroWindow
	{
		public MainWindow()
		{
		}

		protected override void OnClosing(CancelEventArgs e)
		{
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
				case 0:
					flipview.BannerText = "Cupcakes!";
					break;
				case 1:
					flipview.BannerText = "Xbox!";
					break;
				case 2:
					flipview.BannerText = "Chess!";
					break;
			}
		}
	}
}
