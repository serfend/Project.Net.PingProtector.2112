using SGTClientPatchServices;
using System.Runtime.InteropServices;

var path = $"path:{Environment.CurrentDirectory}->{AppDomain.CurrentDomain.BaseDirectory}";
var user = $"user:{Environment.UserName}/domain:{Environment.UserDomainName}";
Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory; // ��ǰĿ¼����Ϊ����Ŀ¼@20211203
File.WriteAllText("test_services_log.log", $"׼������:{path} | {user}");

// ����Ŀ¼��Ӱ�����ʹ��
// ֮�����޷�������������Ϊ�ļ�û�и���ȫ
// System.IO.Directory.SetCurrentDirectory(System.AppDomain.CurrentDomain.BaseDirectory); // via @38 1/40/0744
// System.IO.Directory.SetCurrentDirectory(@"c:\windows\system32"); // Ĭ�Ϲ���Ŀ¼Ϊ

var host = Host.CreateDefaultBuilder(args);
var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);//�жϵ�ǰϵͳ�Ƿ�Ϊwindows

SGTClientPatchServices.Configuration.IsRunning = true;
SGTClientPatchServices.Configuration.IsServicesStop = false;

var hostContainer =
    host.UseWindowsService().
    ConfigureServices(services =>
 {
     services.AddHostedService<Worker>();
     services.AddHostedService<ClientUpdateWorker>();
 });
await hostContainer.Build().RunAsync(SGTClientPatchServices.Configuration.GlobalToken);
Console.WriteLine("main services is about to stop");