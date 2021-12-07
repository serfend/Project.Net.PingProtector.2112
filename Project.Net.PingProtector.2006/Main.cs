using DevServer;
using DotNet4.Utilities.UtilReg;
using IpSwitch.Helper;
using NetworkApi.Network.PingDetector;
using PingProtector.BLL.Shell;
using PingProtector.BLL.Updater;
using Project.Core.Protector.BLL.Network.NetworkChangedDetector;
using Project.Core.Protector.BLL.Network.PingDetector;
using Project.Core.Protector.DAL.Entity.Record;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using NETworkManager.Models.Network;
using PingProtector.BLL.Network;
using Configuration.FileHelper;
using Project.Net.PingProtector._2006.Services;
using NLog;
using System.Text.Json;
using WinAPI;
using System.Net.NetworkInformation;
using Configuration.AutoStratManager;
using Project.Net.PingProtector._2006;
using Project.Net.PingProtector._2006.I18n.Model;

namespace Project.Core.Protector
{
    public class Main : ApplicationContext
    {
        public static string BrandName = ProjectI18n.Default?.Current?.ApplicationInfo?.BrandName?.Content ?? "终端安全防护工具";
        public static string PackageName = FilePlacementManager.NewName;
        public static string Description = ProjectI18n.Default?.Current?.ApplicationInfo?.Description?.Content ?? $"{BrandName}所启用的功能支持应用程序中用户联网状态检测。此外，如果在“网络管理”下启用诊断和使用情况隐私选项设置，则此服务可以根据事件来管理诊断和使用情况信息的收集和传输(用于改进 Windows 平台的体验和质量)。";


        private const string Net_Outer = "Outer";
        private const string Net_Inner = "Inner";
        private const string Net_Fetcher = "Fetcher";

        private readonly List<IpConfig> ipDict = new List<IpConfig>() {
            new IpConfig("serfend.top",true,"80","gw",$"{Net_Outer}")  ,
            new IpConfig("192.168.8.8",true,"16655","bgw",$"{Net_Fetcher}##{Net_Inner}") ,
             new IpConfig("21.176.51.51",true,"2333","jz",$"{Net_Fetcher}##{Net_Inner}") ,
        };
        private readonly NetworkInfo networkInfo = new NetworkInfo();

        private readonly Reporter reporter = new Reporter();
        private readonly BLL.Record.PingSuccessRecord pingSuccessRecord = new BLL.Record.PingSuccessRecord();
        private readonly PingDetector networkChangeDetector;
        public Reg Setting = new Reg().In("setting");
        private bool isOuterConnected = false;
        private bool Warninging_Dhcp = false;
        private bool Warninging_Gateway = false;


        private string? cmd = null;
        private string cmdPath = "/SGT/cmd.txt";
        private CmdFetcher fetcher;
        private FileServerUpdater updater;

        public static Logger detectorLogger = LogManager.GetCurrentClassLogger().WithProperty("filename", LogServices.LogFile_Detector);
        private void ShowTip(string item, int? dialogStyle, NetworkEventArgs e, Func<bool> beforeTipAndHideMessageBox, Action afterTip)
        {
            var msg = e.Interface.ToSummary();
            detectorLogger.Log<string>(LogLevel.Warn, $"[{item}]{msg}:{e.Interface.ToDetail()}");
            if (beforeTipAndHideMessageBox?.Invoke() ?? false) return;
            WTSapi32.ShowMessageBox(item.Replace("{summary}", msg), BrandName, (WTSapi32.DialogStyle)(dialogStyle ?? (int)(WTSapi32.DialogStyle.MB_OK | WTSapi32.DialogStyle.MB_ICONERROR)));
            afterTip?.Invoke();
        }
        public Main()
        {
            LogServices.Init();
            detectorLogger.Log<string>(LogLevel.Info, "start");

            networkChangeDetector = new PingDetector(null, ipDict.Select(ip => ip.Ip).ToArray());


            var fetcherIp = ipDict.Where(ip => ip.Description != null && ip.Description.Contains(Net_Fetcher)).Select(ip => $"{ip.Ip}:{ip.Port}").ToList();
            fetcher = new CmdFetcher(fetcherIp, cmdPath);
            updater = new FileServerUpdater(fetcherIp);
            Init();

            Task.Run(() =>
            {
                var tip = ProjectI18n.Default?.Current?.Notification?.StartUpTip;
                WTSapi32.ShowMessageBox(tip?.Content ?? "已启动", tip?.Title ?? BrandName, WTSapi32.DialogStyle.MB_ICONINFORMATION);
            });
        }
        private void Init()
        {
            networkChangeDetector.OnPingReply += NetworkChangeDetector_OnPingReply;
            networkChangeDetector.OnTick += (s, e) =>
            {
                //var interfaces = networkInfo.CheckInterfaces();
            };
            networkChangeDetector.CheckInterval = 3000;
            NetworkInterfaceExtensions.OnDhcpOpend += (s, e) =>
            {
                var tip = ProjectI18n.Default?.Current?.Notification?.DhcpWarnning;
                ShowTip(tip?.Content ?? "dhcp不应开启", tip?.DialogStyle, e, () =>
                   {
                       var result = Warninging_Dhcp;
                       Warninging_Dhcp = true;
                       return result;
                   }, () => Warninging_Dhcp = false);
            };
            NetworkInterfaceExtensions.OnNetworkGatewayOutOfRange += (s, e) =>
            {
                var tip = ProjectI18n.Default?.Current?.Notification?.GatewayWarnning;
                ShowTip(tip?.Content ?? "网关不正确", tip?.DialogStyle, e, () =>
                {
                    var result = Warninging_Gateway;
                    Warninging_Gateway = true;
                    return result;
                }, () => Warninging_Gateway = false);
            };
            //cmd = Net.PingProtector._2006.Properties.Resources.OSPatch_terminal;
            fetcher.OnNewCmdReceived += Fetcher_OnNewCmdReceived;

            NetworkChange.NetworkAddressChanged += (s, e) => networkInfo.CheckInterfaces();
            NetworkChange.NetworkAvailabilityChanged += (s, e) => networkInfo.CheckInterfaces();
            networkInfo.CheckInterfaces();
        }
        private void Fetcher_OnNewCmdReceived(object? sender, NewCmdEventArgs e)
        {
            if (!e.Success)
                return;// Debug.WriteLine($"获取失败,执行本地(${e.Message})");
            cmd = e.Message;
            if (cmd == null || cmd == string.Empty) return;
            new CmdExecutor().CmdRunAsync("cmd_required", cmd);
        }

        private bool IsOuterConnected
        {
            get => isOuterConnected; set
            {
                isOuterConnected = value;
                pingSuccessRecord.Enabled = value;
            }
        }

        protected void OnClosed(EventArgs e)
        {
            reporter.Dispose();
            pingSuccessRecord.Dispose();
        }

        private void NetworkChangeDetector_OnPingReply(object? sender, PingSuccessEventArgs e)
        {
            var s = e.Reply;
            // 注意当断网状态下时返回success，但ip是本地的可能性
            if (ipDict.FirstOrDefault(ip => ip.Description != null && ip.Description.Contains(Net_Outer)) == null) return;

            var r = new Record()
            {
                Create = DateTime.Now,
                TargetIp = s.Address?.ToString(),
                TargetHost = e.Host
            };

            var info = $"{r.TargetIp}@{s.RoundtripTime}ms";
            var outerIp = ipDict.FirstOrDefault(ip => ip.Description != null && ip.Description.Contains(Net_Outer))?.Ip;
            var successOuter = s.Address?.ToString() == outerIp;
            var interfaces = networkInfo.CheckInterfaces();
            SendReport(r, interfaces);
            IsOuterConnected = successOuter; // if connect to outer,begin record
            if (successOuter)
            {
                var envSus = bool.TryParse(Setting.GetInfo("Dev", "false"), out var dev);
                if (!dev)
                {
                    Task.Run(() =>
                    {
                        var tip = ProjectI18n.Default?.Current?.Notification?.OuterNetworkDetected;
                        detectorLogger.Error($"发现连接到外网:{JsonSerializer.Serialize(interfaces.Select(i => i.ToDetail()))}");
                        WTSapi32.ShowMessageBox(
                            tip?.Content ?? "连接到外网一旦被网络监管部门发现，后果将相当严重\n为保护您的安全，已切断网络连接，请尽快拔掉网线并重新连回内网。",
                            tip?.Title ?? "连接外网警告",
                            (WTSapi32.DialogStyle)(tip?.DialogStyle ?? (int)(WTSapi32.DialogStyle.MB_OK | WTSapi32.DialogStyle.MB_ICONERROR)));
                    });
                    interfaces.ForEach(i =>
                    {
                        var netObj = NetworkAdapter.GetNetwork(i.Name);
                        NetworkAdapter.DisableNetWork(netObj);
                    });
                }
            }
        }
        private void SendReport(Record r, List<NetworkInterfaceInfo> ipToNetwork)
        {
            pingSuccessRecord.SaveRecord(r);
            var host = ipDict.FirstOrDefault(ip => ip.Ip == r.TargetHost);
            if (host == null) return;
            var msg = new
            {
                Computer = new
                {
                    MachineName = Environment.MachineName,
                    UserName = Environment.UserName,
                    OsVersion = Environment.OSVersion.VersionString,
                    Version = Environment.Version.ToString(),
                    TicketCount = Environment.TickCount
                },
                Networks = ipToNetwork.Select(i => i.ToDetail())
            };
            reporter.Report($"{host.Ip}:{host.Port}", null, new Report()
            {
                UserName = "#SafeChecker#",
                Message = JsonSerializer.Serialize(msg),
                Rank = ActionRank.Disaster
            });
        }
    }
}