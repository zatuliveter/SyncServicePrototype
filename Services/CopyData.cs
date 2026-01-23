//using Serilog;

//namespace TaskFactory.Common;

//public class CopyData (ILogger logger)
//	: TaskBase<CopyTableDefinition>
//{
//	private readonly ILogger _logger = logger.ForContext<CopyData>();

//	protected override async Task ExecuteAsync(CopyTableDefinition args, string taskId, IPipelineContext context, CancellationToken ct)
//	{
//		_logger.BindProperty("PipelineContext", context, true, out _);

//		_logger.Information("{pipelineName}.{taskId}: Loading orders...", context.PipelineName, taskId);

//		await Task.Delay(new Random().Next(3000), ct);
//	}
//}
