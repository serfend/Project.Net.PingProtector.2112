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

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("services execute : {time}", DateTimeOffset.Now);
            Application.Run(new Project.Core.Protector.Main());
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
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