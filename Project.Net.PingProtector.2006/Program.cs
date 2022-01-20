using Common.Extensions;
using Configuration.AutoStratManager;
using PermissionManager;
using Project.Core.Protector;
using Project.Net.PingProtector._2006.I18n;
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
			try
			{
				AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
				Application.ThreadException += Application_ThreadException;
				StartPermissionCheck();
				//new FilePlacementManager().Check(); // ʹ�ð�װ��ģʽ����
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
				WTSapi32.ShowMessageBox(result, "�����쳣", WTSapi32.DialogStyle.MB_OK | WTSapi32.DialogStyle.MB_ICONERROR);
			}
		}

		/// <summary>
		/// ���Ȩ�ޣ�������������ʾ
		/// </summary>
		private static void StartPermissionCheck()
		{
			var nowPermission = WindowsIdentity.GetCurrent().GetClaims();
			var token = $"{Environment.UserName}/{Environment.UserDomainName}";
			var desc = $"{string.Join(',', nowPermission)}:{token}";

			var tip = ProjectI18n.Default?.Current?.Notification?.StartUpTip;
			if (!Environment.UserName.Contains("$"))
			{
				new PermissionChecker().UseSystem(); // ��ʹ������Ȩ��ʱ����$��ʶ
				return;
			}
			IntPtr.Zero.ShowMessageBox($"{tip?.Content ?? "������"}{appUpdater.CurrentVersion}@{selfInstaceId}\n{desc}", tip?.Title ?? Project.Core.Protector.Main.BrandName, WTSapi32.DialogStyle.MB_ICONINFORMATION);
		}

		private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
		{
			WTSapi32.ShowMessageBox(e.Exception.ToSummary(), "�̴߳���", WTSapi32.DialogStyle.MB_OK | WTSapi32.DialogStyle.MB_ICONERROR);
		}

		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			WTSapi32.ShowMessageBox(e?.ExceptionObject?.ToString() ?? "����Ϣ", "ϵͳ����", WTSapi32.DialogStyle.MB_OK | WTSapi32.DialogStyle.MB_ICONERROR);
		}
	}
}