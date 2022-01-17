namespace SignalRCommunicator.Events
{
	public class ExceptionEventArgs : EventArgs
	{
		public Exception? Exception { get; set; }

		public ExceptionEventArgs(Exception? ex)
		{
			this.Exception = ex;
		}
	}
}