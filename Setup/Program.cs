

using Common.PowershellHelper;
using DotNet4.Utilities.UtilReg;
using PingProtector.BLL.Shell;
using Serilog;
using Setup;
using System.Windows.Forms;

namespace Setup
{
    public static class Program
    {

        public static string brand = Project.Core.Protector.Main.BrandName;
        public static string packageName = Project.Core.Protector.Main.PackageName;
        public static string description = Project.Core.Protector.Main.Description;
        public static void Main(string[] args)
        {
            var targetPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));

            LogInitializer.Init();
            Log.Warning($"即将安装{brand},按下回车键继续。");
            Console.ReadLine();

            Log.Warning($"开始迁移文件");
            var migrator = new FileMover(packageName, null, targetPath);
            migrator.OnFileMigrate += (s, e) =>
            {
                if (e.FileStatus.HasFlag(WinAPI.FileHandlerExtensions.FileStatus.IsOccupy))
                {

                }
            };
            migrator.Migrate();

            Log.Warning($"开始安装服务");
            //DSAPI.文件.删除Windows服务(packageName);
            var exePath = Path.Combine(migrator.DstPath, $"SGTClientPatchServices.exe");
            DSAPI.文件.将可执行程序注册为Windows服务(exePath, packageName, brand, DSAPI.文件.服务启动方式.自动, "s");
            //var cmd = $"sc create {packageName} binPath= \"{exePath}\" DisplayName= {brand} start= auto";
            //new CmdExecutor() { EnableRedirectStandardInput = true, EnableRedirectStandardOutput = true }.CmdRun("注册服务", cmd, null, true);
            //new CmdExecutor() { EnableRedirectStandardInput = true, EnableRedirectStandardOutput = true }.CmdRun("服务信息", $"sc description {packageName} \"{description}\"", null, true);
            new Reg($"SYSTEM\\ControlSet001\\Services\\{packageName}", RegDomain.LocalMachine).SetInfo("Description", description);
            Log.Warning("完成安装");
            if (MessageBox.Show(null, "安装完成，需要重启", "完成", MessageBoxButtons.OKCancel) == DialogResult.OK)
                PowerShellHelper.ExecuteCommand($"shutdown -r -t 0");
        }
    }
}