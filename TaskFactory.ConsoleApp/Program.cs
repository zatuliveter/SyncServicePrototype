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


// DI container resolves all constructor arguments automatically
IPipelineRunner runner = provider.GetRequiredService<IPipelineRunner>();
ILogger logger = provider.GetRequiredService<ILogger>();

IPipeline pipeline = new DailyProcessingPipeline();

PipelineRunResult result = await runner.RunAsync(
	pipeline,
	parallelTaskCount: 10,
	PipelineFailureMode.SkipDependentTasks,
	CancellationToken.None
);


if (!result.IsSuccess)
{
	throw new AggregateException(
		"Pipeline failed",
		result.Tasks.Where(t => t.Value.Status == TaskExecutionStatus.Failed).Select(t => t.Value.Error!)
	);
}

logger.Information("Run duration: {duration}", result.Duration);
logger.Information("Done.");



