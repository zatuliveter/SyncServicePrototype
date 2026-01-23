
namespace TaskFactory;

internal sealed class TaskNode
{
	public required PipelineItemBase Item { get; init; }
	public required HashSet<string> RemainingDependencies { get; init; }
	public required List<string> Dependents { get; init; }

	public TaskExecutionStatus Status;
}
