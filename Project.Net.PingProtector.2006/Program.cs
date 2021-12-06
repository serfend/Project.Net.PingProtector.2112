using Configuration.AutoStratManager;
using Project.Core.Protector;
using WinAPI;

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
            try
            {
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                Application.ThreadException += Application_ThreadException;
                Application.Run(new Main());
            }
            catch (Exception ex)
            {
                WTSapi32.ShowMessageBox(ex.Message + "\n" + ex.StackTrace, "主线异常", WTSapi32.DialogStyle.MB_OK | WTSapi32.DialogStyle.MB_ICONERROR);

            }
        }


        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            WTSapi32.ShowMessageBox($"{e.Exception.Message}\n{e.Exception.StackTrace}", "线程错误", WTSapi32.DialogStyle.MB_OK | WTSapi32.DialogStyle.MB_ICONERROR);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            WTSapi32.ShowMessageBox(e.ExceptionObject.ToString(), "系统错误", WTSapi32.DialogStyle.MB_OK | WTSapi32.DialogStyle.MB_ICONERROR);
        }
    }
}