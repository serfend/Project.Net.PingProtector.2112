using M2.NSudo;
using System.Diagnostics;
using System.Security.Principal;

namespace PermissionManager
{
	public class PermissionChecker
	{
		public bool UseSystem()
		{
			var nowPermission = WindowsIdentity.GetCurrent().GetClaims();
			if (nowPermission.Contains(WindowsBuiltInRole.SystemOperator)) return true;
			var currentPath = Process.GetCurrentProcess()?.MainModule?.FileName;
			var currentDirectory = Path.GetDirectoryName(currentPath);
			// 当终端存在UAC时将报以下异常
			// message:并非所有被引用的特权或组都分配给呼叫方。
			new NSudoInstance().CreateProcess(
					NSUDO_USER_MODE_TYPE.TRUSTED_INSTALLER,
					NSUDO_PRIVILEGES_MODE_TYPE.ENABLE_ALL_PRIVILEGES,
					NSUDO_MANDATORY_LABEL_TYPE.SYSTEM,
					NSUDO_PROCESS_PRIORITY_CLASS_TYPE.NORMAL,
					NSUDO_SHOW_WINDOW_MODE_TYPE.DEFAULT,
					0, true, currentPath, currentDirectory);
			return false;
		}
	}
}