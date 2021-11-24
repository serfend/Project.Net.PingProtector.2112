using System;
using System.Net.NetworkInformation;

namespace Project.Core.Protector.BLL.Network.PingDetector
{
	public class PingSuccessEventArgs : EventArgs
	{
		public PingSuccessEventArgs(PingReply reply)
		{
			Reply = reply;
		}

		public PingReply Reply { get; }
	}
}