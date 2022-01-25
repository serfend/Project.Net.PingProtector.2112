// See https://aka.ms/new-console-template for more information
using SignalRCommunicator;

Console.WriteLine("Hello, World!");
var s = new SignalrCommunicator("192.168.8.196:2333");
//var s = new SignalrCommunicator("serfend.top");
s.OnConnectionRebuild += (sender, e) =>
{
	s.ReportClientInfo(new DevServer.Report<string>() { });
};
while (!s.ReportClientInfo(new DevServer.Report<string>() { }).Result)
{
	Thread.Sleep(1000);
}

Thread.Sleep(10000);