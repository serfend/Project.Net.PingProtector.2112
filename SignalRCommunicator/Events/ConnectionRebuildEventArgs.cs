namespace SignalRCommunicator.Events
{
	public class ConnectionRebuildEventArgs : EventArgs
	{
		public ConnectionRebuildEventArgs(string? newId)
		{
			NewId = newId;
		}

		/// <summary>
		/// connection id
		/// </summary>
		public string? NewId { get; }
	}
}