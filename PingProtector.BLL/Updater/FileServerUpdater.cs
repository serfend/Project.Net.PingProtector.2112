using DotNet4.Utilities.UtilReg;
using NETworkManager.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;

namespace PingProtector.BLL.Updater
{
    public class FileServerUpdater
    {
        public const string UpdatePath = "/file/load?filepath=/tools/sgt&filename=everynet.exe";
        private const string DownLoadPath = "/file/staticFile/";
        private List<string> Host { get; set; }
        private Reg updateRecord = new Reg().In("setting");

        public event EventHandler<NewVersionEventArgs> OnNewVersion;

        public Timer checkUpdate;

        public FileServerUpdater(List<string> host)
        {
            this.Host = host;
            CheckUpdate();
            checkUpdate = new Timer(new TimerCallback(o => CheckUpdate()), null, 0, 60000);
            OnNewVersion += (s, e) => { };
            OnNewVersion?.Invoke(this, new NewVersionEventArgs());
        }

        public void CheckUpdate()
        {
            var currentUpdateInfo = updateRecord.GetInfo("CurrentUpdate", "2020-1-1");
            var success = DateTime.TryParse(currentUpdateInfo, out var currentUpdate);
            if (!success) currentUpdate = DateTime.Now.AddYears(-1);
            foreach (var host in Host)
            {
                var message = new HttpRequestMessage(HttpMethod.Get, new Uri($"http://{host}{UpdatePath}"));
                if (RunUpdate(message, currentUpdate, host)) break;
            }
        }

        private bool RunUpdate(HttpRequestMessage message, DateTime currentUpdate, string host)
        {
            using (var http = new HttpClient())
            {
                try
                {
                    var r = http.SendAsync(message).Result;
                    if (r.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var content = r.Content.ReadAsStringAsync().Result;
                        var item = JsonConvert.DeserializeObject(content) as Newtonsoft.Json.Linq.JObject;
                        var file = item?.SelectToken("data.file");
                        if (file == null) return false;
                        var lastModefy = file.SelectToken("lastModefy")?.ToString();
                        DateTime.TryParse(lastModefy, out var lastModify);
                        if (currentUpdate < lastModify)
                        {
                            var id = file.SelectToken("id")?.ToString() ?? "default";
                            UpdateVersion(host, id, lastModify);
                            return true;
                        }
                    }
                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        private void UpdateVersion(string host, string id, DateTime lastModify)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, new Uri($"http://{host}{DownLoadPath}{id}"));
            using (var http = new HttpClient())
            {
                var file = http.SendAsync(message).Result;
                var sr = file.Content.ReadAsByteArrayAsync().Result;
                using var tmpFile = File.OpenWrite("./update.tmp");
                tmpFile.Write(sr, 0, sr.Length);
                OnNewVersion?.Invoke(this, new NewVersionEventArgs() { FileName = id, LastModify = lastModify, Length = sr.Length });
            }
            updateRecord.SetInfo("CurrentUpdate", lastModify.ToString());
            var cmd = new StringBuilder();
            var exe = Process.GetCurrentProcess().ProcessName;
            var path = ConfigurationManager.Current.ExecutionPath;
            var fullPath = ConfigurationManager.Current.ApplicationFullName;
            cmd.AppendLine($"del {fullPath}");
            cmd.AppendLine($"copy /y {path}\\update.tmp {path}\\everynet.exe");
            cmd.AppendLine($"start  {path}\\everynet.exe");
            cmd.AppendLine($"del {path}\\update.tmp");
            RunCmdOuter(cmd.ToString());
        }

        public void RunCmdOuter(string cmd)
        {
            var lines = new StringBuilder();
            lines.AppendLine("timeout /T 3 /NOBREAK");
            lines.AppendLine(cmd);
            lines.AppendLine("del update.bat");
            File.WriteAllText("update.bat", lines.ToString());
            Process.Start(new ProcessStartInfo()
            {
                FileName = "update.bat",
                UseShellExecute = true,
                CreateNoWindow = true,
            });
            Environment.Exit(0);
        }
    }

    public class NewVersionEventArgs : EventArgs
    {
        public DateTime LastModify { get; set; }
        public int Length { get; set; }
        public string FileName { get; set; }
    }
}