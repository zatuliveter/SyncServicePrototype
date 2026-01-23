
using Serilog;

namespace TaskFactory.ConsoleApp;


public class CustomLoadOrdersTask (ILogger logger) : ITask
{
	public Task ProcessAsync(object? parameters, string taskId, IPipelineContext context, CancellationToken ct)
	{
		logger.Information("Executing CustomLoadOrdersTask.");
		return Task.CompletedTask;
	}
}