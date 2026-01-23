namespace TaskFactory;

/// <summary>
/// Task implementations must be stateless. All state must be external.
/// </summary>
public interface ITask
{
	Task ProcessAsync(object? parameters, string taskId, IPipelineContext context, CancellationToken ct);
}
