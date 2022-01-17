using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace WinAPI
{
	public static partial class WTSapi32
	{
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern int WTSGetActiveConsoleSessionId();

		[DllImport("wtsapi32.dll", SetLastError = true)]
		public static extern bool WTSSendMessage(
			IntPtr hServer,
			int SessionId,
			String pTitle,
			int TitleLength,
			String pMessage,
			int MessageLength,
			int Style,
			int Timeout,
			out int pResponse,
			bool bWait);

		public static IntPtr WTS_CURRENT_SERVER_HANDLE = IntPtr.Zero;

		public static int ShowMessageBox(this IntPtr hwnd, string message, string title, DialogStyle flag = 0)
		{
			WTSSendMessage(
			hwnd,
			WTSGetActiveConsoleSessionId(),
			title, title.Length,
			message, message.Length,
			(int)flag, 0, out var resp, false);
			return resp;
		}

		[Obsolete($"请使用{nameof(ShowMessageBox)}IntPtr来操作")]
		public static int ShowMessageBox(string message, string title, DialogStyle flag = 0) => WTS_CURRENT_SERVER_HANDLE.ShowMessageBox(message, title, flag);

		[Flags]
		public enum DialogStyle : long
		{
			None = -1,
			Default = 0,
			MB_YESNOCANCEL = 0x00000003L,
			MB_YESNO = 0x00000004L,
			MB_RETRYCANCEL = 0x00000005L,
			MB_OKCANCEL = 0x00000001L,
			MB_OK = 0x00000000L,
			MB_HELP = 0x00004000L,
			MB_CANCELTRYCONTINUE = 0x00000006L,
			MB_ABORTRETRYIGNORE = 0x00000002L,
			MB_ICONWARNING = 0x00000030L,
			MB_ICONINFORMATION = 0x00000040L,
			MB_ICONQUESTION = 0x00000020L,
			MB_ICONERROR = 0x00000010L
		}
	}

	public static partial class WTSapi32
	{
		public enum CreateProcessResult
		{
			Success = 0,
			WTSQueryUserTokenFail = 1,
			DuplicateTokenExFail = 2,
			CreateEnvironmentBlockFail = 4,
			CreateUserProcessFail = 8
		}

		public static (CreateProcessResult, int) CreateProcess(this FileInfo processFile)
		{
			if (processFile?.DirectoryName == null) throw new ArgumentNullException(nameof(processFile));
			var app = processFile.FullName;
			var path = processFile.DirectoryName;
			bool result;
			IntPtr hDupedToken = IntPtr.Zero;
			var pi = new PROCESS_INFORMATION();
			var sa = new SECURITY_ATTRIBUTES();
			sa.Length = Marshal.SizeOf(sa);
			var si = new STARTUPINFO();
			si.cb = Marshal.SizeOf(si);
			int dwSessionID = WTSGetActiveConsoleSessionId();
			result = WTSQueryUserToken(dwSessionID, out var hToken);
			if (!result) return (CreateProcessResult.WTSQueryUserTokenFail, Marshal.GetLastWin32Error());
			result = DuplicateTokenEx(hToken, GENERIC_ALL_ACCESS, ref sa, (int)SECURITY_IMPERSONATION_LEVEL.SecurityIdentification, (int)TOKEN_TYPE.TokenPrimary, ref hDupedToken);
			if (!result) return (CreateProcessResult.DuplicateTokenExFail, Marshal.GetLastWin32Error());
			result = CreateEnvironmentBlock(out var lpEnvironment, hDupedToken, false);
			if (!result) return (CreateProcessResult.CreateEnvironmentBlockFail, Marshal.GetLastWin32Error());
			result = CreateProcessAsUser(hDupedToken, app, string.Empty, ref sa, ref sa, false, 0, IntPtr.Zero, path, ref si, ref pi);
			if (!result) return (CreateProcessResult.CreateUserProcessFail, Marshal.GetLastWin32Error());
			if (pi.hProcess != IntPtr.Zero)
				CloseHandle(pi.hProcess);
			if (pi.hThread != IntPtr.Zero)
				CloseHandle(pi.hThread);
			if (hDupedToken != IntPtr.Zero)
				CloseHandle(hDupedToken);
			return (CreateProcessResult.Success, 0);
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct STARTUPINFO
		{
			public int cb;
			public string lpReserved;
			public string lpDesktop;
			public string lpTitle;
			public int dwX;
			public int dwY;
			public int dwXSize;
			public int dwXCountChars;
			public int dwYCountChars;
			public int dwFillAttribute;
			public int dwFlags;
			public short wShowWindow;
			public short cbReserved2;
			public IntPtr lpReserved2;
			public IntPtr hStdInput;
			public IntPtr hStdOutput;
			public IntPtr hStdError;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct PROCESS_INFORMATION
		{
			public IntPtr hProcess;
			public IntPtr hThread;
			public int dwProcessID;
			public int dwThreadID;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct SECURITY_ATTRIBUTES
		{
			public int Length;
			public IntPtr lpSecurityDescriptor;
			public bool bInheritHandle;
		}

		public enum SECURITY_IMPERSONATION_LEVEL
		{
			SecurityAnonymous,
			SecurityIdentification,
			SecurityImpersonation,
			SecurityDelegation
		}

		public enum TOKEN_TYPE
		{
			TokenPrimary = 1,
			TokenImpersonation
		}

		public const int GENERIC_ALL_ACCESS = 0x10000000;

		[DllImport("kernel32.dll", SetLastError = true,
			CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
		public static extern bool CloseHandle(IntPtr handle);

		[DllImport("advapi32.dll", SetLastError = true,
			CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
		public static extern bool CreateProcessAsUser(
			IntPtr hToken,
			string lpApplicationName,
			string lpCommandLine,
			ref SECURITY_ATTRIBUTES lpProcessAttributes,
			ref SECURITY_ATTRIBUTES lpThreadAttributes,
			bool bInheritHandle,
			int dwCreationFlags,
			IntPtr lpEnvrionment,
			string lpCurrentDirectory,
			ref STARTUPINFO lpStartupInfo,
			ref PROCESS_INFORMATION lpProcessInformation);

		[DllImport("advapi32.dll", SetLastError = true)]
		public static extern bool DuplicateTokenEx(
			IntPtr hExistingToken,
			int dwDesiredAccess,
			ref SECURITY_ATTRIBUTES lpThreadAttributes,
			int ImpersonationLevel,
			int dwTokenType,
			ref IntPtr phNewToken);

		[DllImport("wtsapi32.dll", SetLastError = true)]
		public static extern bool WTSQueryUserToken(
			int sessionId,
			out IntPtr Token);

		[DllImport("userenv.dll", SetLastError = true)]
		private static extern bool CreateEnvironmentBlock(
			out IntPtr lpEnvironment,
			IntPtr hToken,
			bool bInherit);
	}
}