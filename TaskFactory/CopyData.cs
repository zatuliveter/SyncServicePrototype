using Serilog;

namespace TaskFactory;

public class CopyData (ILogger logger)
	: TaskBase<CopyDataDefinition>
{
	protected override Task ExecuteAsync(CopyDataDefinition args, IPipelineContext context, CancellationToken ct)
	{
		logger.Information("Copying from {SourceTable}", args.SourceTableName);
		return Task.CompletedTask;
	}
}
