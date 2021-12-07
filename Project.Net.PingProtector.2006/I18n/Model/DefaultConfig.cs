using Project.Core.Protector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Project.Net.PingProtector._2006.I18n.Model
{
    public static class DefaultConfig
    {
        public static void Initilize() {

            //var t = new Dictionary<string, Config>();
            //t.Add("zh_cn", new Config()
            //{

            //    ApplicationInfo = new ApplicationInfo()
            //    {
            //        BrandName = new I18nContentItem() { Content = Main.BrandName },
            //        Description = new I18nContentItem() { Content = Main.Description },
            //        PackageName = new I18nContentItem() { Content = Main.PackageName },
            //    },
            //    Notification = new Notification()
            //    {
            //        DhcpWarnning = new I18nWarningItem() { Content = "检测到开启了DHCP，为了您的安全，即将关闭:{summary}" },
            //        GatewayWarnning = new I18nWarningItem() { Content = "检测到使用了危险的网关，为了您的安全，即将关闭:{summary}" },
            //        Ipv6Warnning = new I18nWarningItem() { Content = "检测到启用了Ipv6，为了您的安全，即将关闭:{summary}" },
            //        StartUpTip = new I18nContentItem() { Content = "已启动保护，SGT团队为您的安全保驾护航！" },
            //        OuterNetworkDetected = new I18nWarningItem() { Content = "连接到外网一旦被网络监管部门发现，后果将相当严重\n为保护您的安全，已切断网络连接，请尽快拔掉网线并重新连回内网。", Title = "连接外网警告" }
            //    }
            //});
            //var content = JsonSerializer.Serialize(t);
            //ProjectI18n.Default.Config.Save(content);
        }
    }
}
