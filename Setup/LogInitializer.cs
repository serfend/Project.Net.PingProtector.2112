using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
namespace Setup
{
    internal class LogInitializer
    {
        /// <summary>
        /// 初始化日志目录并返回目录
        /// </summary>
        /// <param name="logfile"></param>
        /// <returns></returns>
        public static string Init(string? logfile = null)
        {
            logfile = logfile ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "setup.log");
            Log.Logger = new LoggerConfiguration()
                            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                            .Enrich.FromLogContext()
                            .WriteTo.Console(LogEventLevel.Verbose, theme: AnsiConsoleTheme.Literate)
                            .WriteTo.File(logfile, LogEventLevel.Verbose,
                                rollingInterval: RollingInterval.Day, retainedFileCountLimit: 90)
                            .CreateLogger();

            return logfile;
        }
    }
}
