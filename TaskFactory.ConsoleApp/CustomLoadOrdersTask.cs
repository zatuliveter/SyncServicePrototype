
using Serilog;

namespace TaskFactory.ConsoleApp;


public class CustomLoadOrdersTask(ILogger logger) : ITask
{
	private readonly ILogger _logger = logger.ForContext<CustomLoadOrdersTask>();

	public async Task ProcessAsync(object? parameters, string taskId, IPipelineContext context, CancellationToken ct)
	{

		_logger.Information("{pipelineName}.{taskId}: Loading orders...", context.PipelineName, taskId);

		await Task.Delay(new Random().Next(3000), ct);
	}
}