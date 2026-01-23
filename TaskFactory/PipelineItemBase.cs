namespace TaskFactory;

public abstract record PipelineItemBase
{
	public string Id { get; }
	public Type TaskType { get; }
	public IReadOnlyCollection<string> DependsOn { get; }
	public object? Parameters { get; }

	protected PipelineItemBase(
		string id,
		Type taskType,
		IReadOnlyCollection<string> dependsOn,
		object? parameters
	)
	{
		Id = id;
		TaskType = taskType;
		DependsOn = dependsOn;
		Parameters = parameters;
	}
}
