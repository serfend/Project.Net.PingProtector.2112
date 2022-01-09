
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Security.Principal;

namespace Setup
{
	public static class Program
	{

		public static string brand = Project.Core.Protector.Main.BrandName;
		public static string packageName = Project.Core.Protector.Main.PackageName;
		public static string servicesName = nameof(SGTClientPatchServices);
		public static string description = Project.Core.Protector.Main.Description;
		public static void Main(string[] args)
		{
			var configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("./conf/config.json", false)
				.Build();
			var app = new ServiceCollection()
				.AddLogging(loggingBuilder =>
			{
				loggingBuilder.AddConfiguration(configuration.GetSection("Logging"));
				loggingBuilder.AddConsole();
				loggingBuilder.AddDebug();
			})
				.AddSingleton(configuration)
				.AddSingleton<SetupExecutor>()
				.AddSingleton<FileMover>()
				.BuildServiceProvider();

			app.GetService<SetupExecutor>()?.Run(args);
		}
		
	}
}