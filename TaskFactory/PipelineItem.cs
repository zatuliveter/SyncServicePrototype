namespace TaskFactory;

public sealed record PipelineItem<TTask, TParams> : PipelineItemBase
	where TTask : TaskBase<TParams>
{
	public PipelineItem(
		string id,
		TParams parameters,
		params string[] dependsOn
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
		params string[] dependsOn
	)
		: base(id, typeof(TTask), dependsOn, null)
	{
	}
}