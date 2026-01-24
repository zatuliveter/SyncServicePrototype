using Microsoft.Extensions.DependencyInjection;
using Serilog;
using TaskFactory;
using TaskFactory.ConsoleApp.Pipelines;


// Use ServiceCollection to configure dependencies
IServiceCollection services = new ServiceCollection();

TaskFactory.Bootstrapper.Initialize(services);
TaskFactory.Common.Bootstrapper.Initialize(services);
TaskFactory.ConsoleApp.Bootstrapper.Initialize(services);

IServiceProvider provider = services.BuildServiceProvider();



IPipelineRunner runner = provider.GetRequiredService<IPipelineRunner>();
ILogger logger = provider.GetRequiredService<ILogger>();

PipelineRunResult result = await runner.RunAsync(
	Pipelines.DailyProcessing,
	CancellationToken.None
);


result.ThrowOnError();

logger.Information("Run duration: {duration}", result.Duration);
logger.Information("Done.");



