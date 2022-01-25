namespace SignalRCommunicator
{
	public static class Test
	{
		public static void Main()
		{
			var s = new SignalrCommunicator("serfend.top");
			s.OnConnectionRebuild += (sender, e) =>
			{
				s.ReportClientInfo(new DevServer.Report<string>() { });
			};
			s.ReportClientInfo(new DevServer.Report<string>() { });
			Thread.Sleep(3000);
			s.ReportClientInfo(new DevServer.Report<string>() { });
			Thread.Sleep(10000);
		}
	}
}