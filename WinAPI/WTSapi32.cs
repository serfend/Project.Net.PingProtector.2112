using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WinAPI
{
    public static class WTSapi32
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

        public static int ShowMessageBox(string message, string title, DialogStyle flag = 0)
        {
            WTSSendMessage(
            WTS_CURRENT_SERVER_HANDLE,
            WTSGetActiveConsoleSessionId(),
            title, title.Length,
            message, message.Length,
            (int) flag, 0, out var resp, false);
            return resp;
        }
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
}
