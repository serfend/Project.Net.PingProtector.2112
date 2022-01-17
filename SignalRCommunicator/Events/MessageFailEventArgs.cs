namespace SignalRCommunicator.Events
{
	public class MessageFailEventArgs : ExceptionEventArgs
	{
		public MessageFailEventArgs(Exception? ex) : base(ex)
		{
		}
	}
}