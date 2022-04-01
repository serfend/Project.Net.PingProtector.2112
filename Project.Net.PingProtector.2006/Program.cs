using Common.Extensions;
using Common.NetworkHelper;
using Configuration.AutoStratManager;
using EventLogHandler;
using PermissionManager;
using Project.Core.Protector;
using Project.Net.PingProtector._2006.I18n;
using Project.Net.PingProtector._2006.Services;
using Project.Net.PingProtector._2006.UserConfigration;
using System.Diagnostics;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using WinAPI;

namespace Project.Net.PingProtector._2006
{
	internal static class Program
	{
		private const string UPDATE_FLAG = "update";
		public static Updater.Client.Updater appUpdater = new();
		public static string selfInstaceId = Guid.NewGuid().ToString();

		/// <summary>
		///  The main entry point for the application.
		/// </summary>
		[STAThread]
		private static void Main(string[] args)
		{
			ApplicationConfiguration.Initialize();
			GlobalEventLog.DefaultLogger.WriteEntry("start new", EventLogEntryType.Information);
			try
			{
				AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
				Application.ThreadException += Application_ThreadException;
				NetworkHttpConnectionExtensions.TrustSSL();
				var tip = StartPermissionCheck();
				//new FilePlacementManager().Check(); // ʹ�ð�װ��ģʽ����
				var startManager = new FunctionBySchedule();
				var regStartManager = new FunctionByReg();
				startManager.EnableAsync();
				regStartManager.EnableAsync();
				Application.Run(new Main(tip));
			}
			catch (Exception ex)
			{
				var result = $"�����쳣:\n{ex.ToSummary()}";
				GlobalEventLog.DefaultLogger.WriteEntry(result, EventLogEntryType.Error);
				//WTSapi32.ShowMessageBox(result, "�����쳣", WTSapi32.DialogStyle.MB_OK | WTSapi32.DialogStyle.MB_ICONERROR);
			}
		}

		/// <summary>
		/// ���Ȩ�ޣ�������������ʾ
		/// </summary>
		private static string StartPermissionCheck()
		{
			var nowPermission = WindowsIdentity.GetCurrent().GetClaims();
			var token = $"{Environment.UserName}/{Environment.UserDomainName}";
			var desc = $"{string.Join(',', nowPermission)}:{token}";
			var tip = UnitOfWork.I18N?.Current?.Notification?.StartUpTip;
			if (!Environment.UserName.Contains("$"))
			{
				new PermissionChecker().UseSystem(); // ��ʹ������Ȩ��ʱ����$��ʶ
				Environment.Exit(0);
				return null;
			}
			return $"{tip?.Content ?? "������"}{appUpdater.CurrentVersion}@{selfInstaceId}\n{desc}";
		}

		private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
		{
			var result = $"�̴߳���:\n{e.Exception.ToSummary()}";
			GlobalEventLog.DefaultLogger.WriteEntry(result, EventLogEntryType.Error);
			//WTSapi32.ShowMessageBox(e.Exception.ToSummary(), "�̴߳���", WTSapi32.DialogStyle.MB_OK | WTSapi32.DialogStyle.MB_ICONERROR);
		}

		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			var result = $"ϵͳ����:\n{e?.ExceptionObject?.ToString() ?? "����Ϣ"}";
			GlobalEventLog.DefaultLogger.WriteEntry(result, EventLogEntryType.Error);
			//WTSapi32.ShowMessageBox(result, "ϵͳ����", WTSapi32.DialogStyle.MB_OK | WTSapi32.DialogStyle.MB_ICONERROR);
		}
	}
}