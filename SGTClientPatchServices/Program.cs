using SGTClientPatchServices;
using System.Runtime.InteropServices;

File.WriteAllText("test_services_log.log", $"׼������:{Environment.CurrentDirectory}->{AppDomain.CurrentDomain.BaseDirectory}");
Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
var host = Host.CreateDefaultBuilder(args);
var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);//�жϵ�ǰϵͳ�Ƿ�Ϊwindows
var hostContainer =
    host.UseWindowsService().
    ConfigureServices(services =>
 {
     services.AddHostedService<Worker>();
 });
await hostContainer.Build().RunAsync();