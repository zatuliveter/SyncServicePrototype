
namespace TaskFactory;

internal sealed class TaskNode
{
	public required PipelineItemBase Item { get; init; }

	public required List<string> Dependents { get; init; }

	public int RemainingDependenciesCount;

	public TaskExecutionStatus Status;
}