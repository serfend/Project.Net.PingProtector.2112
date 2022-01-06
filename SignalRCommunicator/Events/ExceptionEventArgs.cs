using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRCommunicator.Events
{
	public class ExceptionEventArgs:EventArgs
	{
		public Exception? Exception { get; set; }
		public ExceptionEventArgs(Exception? ex) {
			this.Exception = ex;
		}
	}
}
