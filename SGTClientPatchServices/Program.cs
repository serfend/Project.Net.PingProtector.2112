using SGTClientPatchServices;
using System.Runtime.InteropServices;

var path = $"path:{Environment.CurrentDirectory}->{AppDomain.CurrentDomain.BaseDirectory}";
var user = $"user:{Environment.UserName}/domain:{Environment.UserDomainName}";
Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory; // 当前目录设置为程序集目录@20211203
File.WriteAllText("test_services_log.log", $"{DateTime.Now} :准备启动:{path} | {user}");

// 程序集目录不影响最后使用
// 之所以无法启动服务，是因为文件没有复制全
// System.IO.Directory.SetCurrentDirectory(System.AppDomain.CurrentDomain.BaseDirectory); // via @38 1/40/0744
// System.IO.Directory.SetCurrentDirectory(@"c:\windows\system32"); // 默认工作目录为

var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);//判断当前系统是否为windows


RegisterConfigration.Configuration.IsRunning = true;
RegisterConfigration.Configuration.IsServicesStop = false;

var host = Host.CreateDefaultBuilder(args);
var hostContainer =
	host.
	UseWindowsService().
	ConfigureServices(services =>
 {
	 services.AddHostedService<Worker>();
	 services.AddHostedService<ClientUpdateWorker>();
 });
await hostContainer.Build().RunAsync(RegisterConfigration.Configuration.GlobalToken);

Console.WriteLine("main services is about to stop");
RegisterConfigration.Configuration.IsRunning = false;
Thread.Sleep(5000);