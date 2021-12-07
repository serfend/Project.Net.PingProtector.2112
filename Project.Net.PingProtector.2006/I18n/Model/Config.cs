using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Net.PingProtector._2006.I18n.Model
{
    public class Config
    {
        public Notification? Notification { get; set; }
        public ApplicationInfo? ApplicationInfo { get; set; }
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
