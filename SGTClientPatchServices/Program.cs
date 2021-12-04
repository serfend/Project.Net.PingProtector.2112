using SGTClientPatchServices;
using System.Runtime.InteropServices;

var host = Host.CreateDefaultBuilder(args);
var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);//�жϵ�ǰϵͳ�Ƿ�Ϊwindows
var hostContainer = isWindows ?
    host.UseWindowsService() :
    host.ConfigureServices(services =>
 {
     services.AddHostedService<Worker>();
 }); ;
await hostContainer.Build().RunAsync();