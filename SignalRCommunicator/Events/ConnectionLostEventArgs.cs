using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRCommunicator.Events
{
	public class ConnectionLostEventArgs:ExceptionEventArgs
	{
		public ConnectionLostEventArgs(Exception? ex) : base(ex) { }
	}
}
