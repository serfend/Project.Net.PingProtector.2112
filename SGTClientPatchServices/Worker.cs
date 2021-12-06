using System.Diagnostics;
using System.Windows.Forms;

namespace SGTClientPatchServices
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }
        private Process NewProcess()
        {
            return new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = Path.Join(AppDomain.CurrentDomain.BaseDirectory, $"{Project.Core.Protector.Main.PackageName}.exe"),
                    UseShellExecute = true,
                },
                
            };
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("services execute : {time}", DateTimeOffset.Now);
            Process? process = null;
            while (!stoppingToken.IsCancellationRequested)
            {
                var shouldStart = process?.HasExited ?? true;
                if (shouldStart)
                {
                    _logger.LogInformation("start new process : {time}", DateTimeOffset.Now);
                    process = NewProcess();
                    process.Start();
                }
                await Task.Delay(1000, stoppingToken);
            }
            if (!(process?.HasExited ?? true)) process?.Kill();
            _logger.LogInformation("services process stop : {time}", DateTimeOffset.Now);
        }
        public override Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("services start : {time}", DateTimeOffset.Now);
            return base.StartAsync(stoppingToken);
        }
        public override Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("services stop : {time}", DateTimeOffset.Now);
            return base.StopAsync(stoppingToken);
        }

    }
}