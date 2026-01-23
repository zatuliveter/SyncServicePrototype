namespace TaskFactory;

public class CopyData : TaskBase<CopyDataDefinition>
{
	protected override Task ExecuteAsync(CopyDataDefinition args, IPipelineContext context, CancellationToken ct)
	{
		throw new NotImplementedException();
	}
}
