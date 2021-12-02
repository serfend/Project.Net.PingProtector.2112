using System;
using System.Net.NetworkInformation;

namespace Project.Core.Protector.BLL.Network.PingDetector
{
	public class PingSuccessEventArgs : EventArgs
	{
		public PingSuccessEventArgs(PingReply reply,string host)
		{
			Reply = reply;
            Host = host;
        }

		public PingReply Reply { get; }
        public string Host { get; }
    }
}