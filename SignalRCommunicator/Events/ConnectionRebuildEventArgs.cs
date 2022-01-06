using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRCommunicator.Events
{
	public class ConnectionRebuildEventArgs: EventArgs
	{
		public ConnectionRebuildEventArgs(string? newId) {
			NewId = newId;
		}
		/// <summary>
		/// connection id
		/// </summary>
		public string? NewId { get; }
	}
}
