using Configuration.AutoStratManager;
using Project.Core.Protector;

namespace Project.Net.PingProtector._2006
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            //new FilePlacementManager().Check(); // 使用安装包模式部署
            var startManager = new FunctionBySchedule();
            var regStartManager = new FunctionByReg();
            startManager.EnableAsync();
            regStartManager.EnableAsync();
            Application.Run(new Main());
        }
    }
}