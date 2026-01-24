namespace TaskFactory;

public sealed record PipelineItem<TTask, TParams> : PipelineItemBase
	where TTask : TaskBase<TParams>
{
	public PipelineItem(
		string id,
		TParams parameters,
		string[]? dependsOn = null
	)
		: base(id, typeof(TTask), dependsOn, parameters)
	{
	}
}


public sealed record PipelineItem<TTask> : PipelineItemBase
	where TTask : ITask
{
	public PipelineItem(
		string id,
		string[]? dependsOn = null
	)
		: base(id, typeof(TTask), dependsOn, null)
	{
	}
}