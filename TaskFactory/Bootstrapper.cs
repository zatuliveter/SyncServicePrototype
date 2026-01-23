using Microsoft.Extensions.DependencyInjection;
namespace TaskFactory;

public static class Bootstrapper
{
	public static void Initialize(this IServiceCollection container)
	{
		_ = container
			.AddSingleton<IPipelineValidator, PipelineValidator>()
			.AddSingleton<IPipelineRunner, PipelineRunner>();
	}
}
