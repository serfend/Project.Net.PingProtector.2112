using Newtonsoft.Json;
using System;
using System.Runtime.InteropServices;

namespace PacketSnifferNET
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public struct PacketData
	{
		public int length;
		public int s_port;
		public int d_port;

		[MarshalAs(UnmanagedType.LPStr)]
		public string protocal;

		[MarshalAs(UnmanagedType.LPStr)]
		public string s_addr;

		[MarshalAs(UnmanagedType.LPStr)]
		public string d_addr;
	}

	public class Sniffer : IDisposable
	{
		private readonly RawSocket myRawSock = new RawSocket();

		public Sniffer()
		{
			if (OperatingSystem.IsWindows())
			{
				string IPString = "";
				myRawSock.CreateAndBindSocket(IPString);
				myRawSock.PacketArrival += new RawSocket.PacketArrivedEventHandler(DataArrival);
				myRawSock.KeepRunning = true;
				myRawSock.Run();
			}
			else
			{
				//If your are running under macOS, make sure you pass the correct interface name.
				capture("en0", CSCallbackFun);
			}
		}

		[DllImport(@"packetsniffer.so", EntryPoint = "capture", CallingConvention = CallingConvention.StdCall)]
		public static extern int capture(string iface, [MarshalAs(UnmanagedType.FunctionPtr)] CallbackFun callback_f);

		public delegate void CallbackFun(IntPtr data);

		private static void CSCallbackFun(IntPtr data)
		{
			PacketData p = (PacketData)Marshal.PtrToStructure(data, typeof(PacketData));
			RawSocket.PacketArrivedEventArgs args = new RawSocket.PacketArrivedEventArgs
			{
				MessageLength = (uint)p.length,
				Protocol = p.protocal,
				OriginationPort = p.s_port.ToString(),
				DestinationPort = p.d_port.ToString(),
				OriginationAddress = p.s_addr,
				DestinationAddress = p.d_addr,
				IPVersion = "IPv4"
			};

			DataArrival(null, args);
		}

		private static void DataArrival(Object sender, RawSocket.PacketArrivedEventArgs e)
		{
			Console.WriteLine(JsonConvert.SerializeObject(e));
		}

		public void Dispose()
		{
			myRawSock.Shutdown();
		}
	}
}