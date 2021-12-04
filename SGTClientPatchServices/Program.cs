using SGTClientPatchServices;
using System.Runtime.InteropServices;

var host = Host.CreateDefaultBuilder(args);
var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);//判断当前系统是否为windows
var hostContainer = isWindows ?
    host.UseWindowsService() :
    host.ConfigureServices(services =>
 {
     services.AddHostedService<Worker>();
 }); ;
await hostContainer.Build().RunAsync();