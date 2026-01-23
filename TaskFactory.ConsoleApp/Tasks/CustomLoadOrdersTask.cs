
using Serilog;

namespace TaskFactory.ConsoleApp.Tasks;


public class CustomLoadOrdersTask(ILogger logger) : ITask
{
	private readonly ILogger _logger = logger.ForContext<CustomLoadOrdersTask>();

	public async Task ProcessAsync(object? parameters, string taskId, IPipelineContext context, CancellationToken ct)
	{

		_logger.Information("{pipelineName}.{taskId}: Loading orders...", context.PipelineName, taskId);

		await Task.Delay(1000, ct);
	}
}