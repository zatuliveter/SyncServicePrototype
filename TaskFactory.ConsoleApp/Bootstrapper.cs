using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;
namespace TaskFactory.ConsoleApp;

public static class Bootstrapper
{
	public static void Initialize(this IServiceCollection services)
	{
		Logger logger = new LoggerConfiguration()
					.WriteTo.Console()
					.CreateLogger();

		_ = services
			.AddSingleton<ILogger>(logger)
			.AddTransient<CopyData>()
			.AddTransient<SendEmailTask>()
			.AddTransient<CustomLoadOrdersTask>();
	}
}
