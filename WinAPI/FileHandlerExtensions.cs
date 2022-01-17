using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static WinAPI.Kernel32;
using static WinAPI.Shell32;

namespace WinAPI
{
	public static partial class Shell32
	{
		[DllImport("shell32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		public static extern bool SHFileOperation([In, Out] SHFILEOPSTRUCT str);

		public const int FO_MOVE = 0x1;
		public const int FO_COPY = 0x2;
		public const int FO_DELETE = 0x3;
		public const ushort FOF_NOCONFIRMATION = 0x10;
		public const ushort FOF_ALLOWUNDO = 0x40;

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct SHFILEOPSTRUCT
		{
			public IntPtr hwnd;

			/// <summary>

			/// 设置操作方式，移动：FO_MOVE，复制：FO_COPY，删除：FO_DELETE

			/// </summary>

			public UInt32 wFunc;

			/// <summary>

			/// 源文件路径

			/// </summary>

			public string pFrom;

			/// <summary>

			/// 目标文件路径

			/// </summary>

			public string pTo;

			/// <summary>

			/// 允许恢复

			/// </summary>

			public UInt16 fFlags;

			/// <summary>

			/// 监测有无中止

			/// </summary>

			public int fAnyOperationsAborted;

			public IntPtr hNameMappings;

			/// <summary>

			/// 设置标题

			/// </summary>

			public string lpszProgressTitle;
		}

		[DllImport("kernel32.dll")]
		public static extern IntPtr _lopen(string lpPathName, int iReadWrite);

		[DllImport("kernel32.dll")]
		public static extern bool CloseHandle(IntPtr hObject);

		public const int OF_READWRITE = 2;
		public const int OF_SHARE_DENY_NONE = 0x40;
		public static readonly IntPtr HFILE_ERROR = new IntPtr(-1);
	}

	public static class FileHandlerExtensions
	{
		public static bool Copy(this FileInfo src, string dst, string title = "复制文件")
				=> Copy(src.FullName, dst, title);

		public static bool Copy(this DirectoryInfo src, string dst, string title = "复制文件夹")
		=> Copy(src.FullName, dst, title);

		private static bool Copy(string src, string dst, string title)
		{
			var pm = new SHFILEOPSTRUCT()
			{
				wFunc = Shell32.FO_COPY,
				pFrom = src.TrimEnd('/'),
				pTo = dst.TrimEnd('/'),
				fFlags = Shell32.FOF_ALLOWUNDO,
				lpszProgressTitle = title
			};
			return SHFileOperation(pm);
		}

		public static FileStatus FileCurrentStatus(this FileInfo file)
		{
			if (file == null) return FileStatus.Invalid;
			var vFileName = file.FullName;

			if (!file.Exists)
				return FileStatus.NotExist;
			IntPtr vHandle = _lopen(vFileName, OF_READWRITE | OF_SHARE_DENY_NONE);
			if (vHandle == HFILE_ERROR)
			{
				return FileStatus.IsOccupy;
			}
			CloseHandle(vHandle);
			return FileStatus.None;
		}

		[Flags]
		public enum FileStatus
		{
			None = 0,
			NotExist = 1,
			IsOccupy = 2,
			Invalid = 4
		}
	}
}