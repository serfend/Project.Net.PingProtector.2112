namespace SignalRCommunicator.Events
{
	public class ConnectionLostEventArgs : ExceptionEventArgs
	{
		public ConnectionLostEventArgs(Exception? ex) : base(ex)
		{
		}
	}
}