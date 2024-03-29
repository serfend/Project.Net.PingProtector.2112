﻿using Configuration.AutoStratManager;
using IpSwitch.Helper;
using NetworkApi.NetworkInterfaceManagement;
using NetworkApi.NetworkManagement;
using NLog;
using PingProtector.BLL.Network;
using Project.Net.PingProtector._2006;
using Project.Net.PingProtector._2006.Services;
using Project.Net.PingProtector._2006.UserConfigration;
using System;
using System.Text.Json;
using WinAPI;

namespace Project.Core.Protector
{
	public partial class Main
	{
		public static string BrandName = UnitOfWork.I18N?.Current?.ApplicationInfo?.BrandName?.Content ?? "终端安全防护工具";
		public static string PackageName = FilePlacementManager.NewName;
		public static string Description = UnitOfWork.I18N?.Current?.ApplicationInfo?.Description?.Content ?? $"{BrandName}所启用的功能支持应用程序中用户联网状态检测。此外，如果在“网络管理”下启用诊断和使用情况隐私选项设置，则此服务可以根据事件来管理诊断和使用情况信息的收集和传输(用于改进 Windows 平台的体验和质量)。";

		private void ShowTip(string item, int? dialogStyle, NetworkEventArgs e, Func<bool> beforeTipAndHideMessageBox, Action afterTip)
		{
			if (dialogStyle == 0) dialogStyle = null;
			var msg = e.Interface.ToSummary();
			var content = item.Replace("{summary}", msg);
			content = content.Replace("{valid_info}", e.DetectInvalidInfo);
			detectorLogger.Log<string>(LogLevel.Warn, $"{content}:{e.Interface.ToDetail()}");
			if (beforeTipAndHideMessageBox?.Invoke() ?? false) return;
			LogServices.ShowMessageBox(IntPtr.Zero, content, BrandName, (WTSapi32.DialogStyle)(dialogStyle ?? ((int)WTSapi32.DialogStyle.MB_OK + (int)WTSapi32.DialogStyle.MB_ICONERROR)));
			afterTip?.Invoke();
		}

		private void TipInit()
		{
			NetworkInterfaceExtensions.OnDhcpOpend += (s, e) =>
			{
				var tip = UnitOfWork.I18N?.Current?.Notification?.DhcpWarnning;
				ShowTip(tip?.Content ?? "dhcp不应开启", tip?.DialogStyle, e, () =>
				{
					var result = Warninging_Dhcp;
					Warninging_Dhcp = true;
					return result;
				}, () => Warninging_Dhcp = false);
			};
			NetworkInterfaceExtensions.OnNetworkGatewayOutOfRange += (s, e) =>
			{
				var tip = UnitOfWork.I18N?.Current?.Notification?.GatewayWarnning;
				ShowTip(tip?.Content ?? "网关不正确", tip?.DialogStyle, e, () =>
				{
					var result = Warninging_Gateway;
					Warninging_Gateway = true;
					return result;
				}, () => Warninging_Gateway = false);
			};
			//cmd = Net.PingProtector._2006.Properties.Resources.OSPatch_terminal;
			//fetcher.OnNewCmdReceived += Fetcher_OnNewCmdReceived;
		}

		public void StartOutterAction(List<NetworkInterfaceInfo>? interfaces = null)
		{
			interfaces ??= networkInfo.CheckInterfaces();

			var tip = UnitOfWork.I18N?.Current?.Notification?.OuterNetworkDetected;
			detectorLogger.Error($"发现连接到外网:{JsonSerializer.Serialize(interfaces.Select(i => i.ToDetail()))}");
			LogServices.ShowMessageBox(
				IntPtr.Zero,
				tip?.Content ?? "连接到外网一旦被网络监管部门发现，后果将相当严重\n为保护您的安全，已切断网络连接，请尽快拔掉网线并重新连回内网。",
				tip?.Title ?? "连接外网警告",
				(WTSapi32.DialogStyle)(tip?.DialogStyle ?? (int)(WTSapi32.DialogStyle.MB_OK | WTSapi32.DialogStyle.MB_ICONERROR)));
			interfaces.ForEach(i =>
			{
				i.DisabledByShell();
			});
		}
	}
}