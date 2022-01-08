using Common.Extensions;
using Configuration.AutoStratManager;
using Project.Core.Protector;
using Project.Net.PingProtector._2006.I18n;
using WinAPI;

namespace Project.Net.PingProtector._2006
{
	internal static class Program
	{
		private const string UPDATE_FLAG = "update";
		/// <summary>
		///  The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			ApplicationConfiguration.Initialize();
			try
			{
				//MessageBox.Show($"start up at:args[{args.Length}]");
				//if (args.Length > 0)
				//{
				//	MessageBox.Show($"arg0[{args[0]}]");
				//	var argIsUpdate = args[0].ToLower() == UPDATE_FLAG;
				//	if (argIsUpdate)
				//	{
				//		MessageBox.Show("is update");
				//		Application.Exit();
				//	}
				//}
				AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
				Application.ThreadException += Application_ThreadException;


				//new FilePlacementManager().Check(); // 使用安装包模式部署
				var startManager = new FunctionBySchedule();
				var regStartManager = new FunctionByReg();
				startManager.EnableAsync();
				regStartManager.EnableAsync();
				ProjectI18n.Default = new ProjectI18n(new I18nReader());
				Application.Run(new Main());
			}
			catch (Exception ex)
			{
				var result = ex.ToSummary();
				WTSapi32.ShowMessageBox(result, "主线异常", WTSapi32.DialogStyle.MB_OK | WTSapi32.DialogStyle.MB_ICONERROR);
			}
		}


		private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
		{
			WTSapi32.ShowMessageBox(e.Exception.ToSummary(), "线程错误", WTSapi32.DialogStyle.MB_OK | WTSapi32.DialogStyle.MB_ICONERROR);
		}

		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			WTSapi32.ShowMessageBox(e?.ExceptionObject?.ToString() ?? "无信息", "系统错误", WTSapi32.DialogStyle.MB_OK | WTSapi32.DialogStyle.MB_ICONERROR);
		}
	}
}