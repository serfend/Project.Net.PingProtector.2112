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

            //new FilePlacementManager().Check(); // ʹ�ð�װ��ģʽ����
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
                WTSapi32.ShowMessageBox(ex.Message + "\n" + ex.StackTrace, "�����쳣", WTSapi32.DialogStyle.MB_OK | WTSapi32.DialogStyle.MB_ICONERROR);

            }
        }


        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            WTSapi32.ShowMessageBox($"{e.Exception.Message}\n{e.Exception.StackTrace}", "�̴߳���", WTSapi32.DialogStyle.MB_OK | WTSapi32.DialogStyle.MB_ICONERROR);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            WTSapi32.ShowMessageBox(e?.ExceptionObject?.ToString() ?? "����Ϣ", "ϵͳ����", WTSapi32.DialogStyle.MB_OK | WTSapi32.DialogStyle.MB_ICONERROR);
        }
    }
}