using SGTClientPatchServices;
using System.Runtime.InteropServices;

File.WriteAllText("test_services_log.log", $"准备启动:{Environment.CurrentDirectory}");
var host = Host.CreateDefaultBuilder(args);
var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);//判断当前系统是否为windows
var hostContainer =
    host.UseWindowsService().
    ConfigureServices(services =>
 {
     services.AddHostedService<Worker>();
 });
await hostContainer.Build().RunAsync();