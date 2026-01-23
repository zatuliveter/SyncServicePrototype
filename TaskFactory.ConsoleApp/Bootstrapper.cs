using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;
using TaskFactory.ConsoleApp.Tasks;
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
			.AddTransient<SendEmailTask>()
			.AddTransient<CustomLoadOrdersTask>()
			.AddTransient<CopyDataDemo>();
	}
}
