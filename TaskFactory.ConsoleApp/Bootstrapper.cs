using Microsoft.Extensions.DependencyInjection;
namespace TaskFactory.ConsoleApp;

public static class Bootstrapper
{
	public static void Initialize(this IServiceCollection services)
	{
		_ = services.AddTransient<CopyData>();
			services.AddTransient<SendEmailTask>();
			services.AddTransient<CustomLoadOrdersTask>();
	}
}
