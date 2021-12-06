
using System.Runtime.InteropServices;

namespace WinAPI
{
    public static partial class Shell32
    {
        /// <summary>
        /// 将会触发底层重写，不建议经常用
        /// </summary>
        /// <param name="wEventId"></param>
        /// <param name="uFlags"></param>
        /// <param name="dwItem1"></param>
        /// <param name="dwItem2"></param>
        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        public static extern void SHChangeNotify(int wEventId, int uFlags, int dwItem1, int dwItem2);
        public const int SHCNRF_NewDelivery = 0x8000;
        public const int SHCNE_UPDATEITEM = 0x00002000;
        public const int SHCNF_PATH = 0x0005;
        public const int SHCNF_FLUSH = 0x1000;
    }
    public static class Kernel32
    {
        [DllImport("kernel32", CharSet = CharSet.Ansi)]
        public static extern bool Beep(int frequery, int duration);
    }


}