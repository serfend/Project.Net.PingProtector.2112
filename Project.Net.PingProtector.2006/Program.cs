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

            new FilePlacementManager().Check();
            var startManager = new FunctionBySchedule();
            var regStartManager = new FunctionByReg();
            startManager.EnableAsync();
            regStartManager.EnableAsync();
            Application.Run(new Main());
        }
    }
}