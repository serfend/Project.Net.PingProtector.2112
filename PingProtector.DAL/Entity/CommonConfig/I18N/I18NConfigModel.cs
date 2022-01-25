using PingProtector.DAL.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Net.PingProtector._2006.I18n.Model
{
	public class I18NConfigModel : BaseEntity
	{
		public Notification? Notification { get; set; }
		public ApplicationInfo? ApplicationInfo { get; set; }

		public override object DefaultValue()
		{
			return new I18NConfigModel()
			{
				ApplicationInfo = new()
				{
					BrandName = new() { Disabled = false, Content = "终端安全防护系统", Title = "" },
					Description = new() { Disabled = false, Content = "终端安全防护系统", Title = "" },
					PackageName = new() { Disabled = false, Content = "ClientPatch", Title = "" },
				},
				Notification = new()
				{
					DhcpWarnning = new() { Disabled = false, Content = "DHCP服务危险开启", Title = "危险" },
					GatewayWarnning = new() { Disabled = false, Content = "使用危险网关", Title = "危险" },
					Ipv6Warnning = new() { Disabled = false, Content = "Ipv6危险激活状态", Title = "危险" },
					OuterNetworkDetected = new() { Disabled = false, Content = "发生违规外联！！！", Title = "危险" },
					StartUpTip = new() { Disabled = false, Content = "终端安全防护系统为您网络安全保驾护航", Title = "启动" },
				}
			};
		}
	}

	public class Notification
	{
		public I18nWarningItem? GatewayWarnning { get; set; }
		public I18nWarningItem? Ipv6Warnning { get; set; }
		public I18nWarningItem? DhcpWarnning { get; set; }
		public I18nContentItem? StartUpTip { get; set; }
		public I18nWarningItem? OuterNetworkDetected { get; set; }
	}

	public class ApplicationInfo
	{
		public I18nContentItem? Description { get; set; }
		public I18nContentItem? PackageName { get; set; }
		public I18nContentItem? BrandName { get; set; }
	}

	public class I18nWarningItem : I18nContentItem
	{
		public int? DialogStyle { get; set; } = null;
	}

	public class I18nContentItem
	{
		public bool Disabled { get; set; }
		public string? Title { get; set; }
		public string? Content { get; set; }
	}
}