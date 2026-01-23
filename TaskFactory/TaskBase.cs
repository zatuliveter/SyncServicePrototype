namespace TaskFactory;

public abstract class TaskBase<TParams> : ITask
{
	public Task ProcessAsync(object? parameters, string taskId, IPipelineContext context, CancellationToken ct)
	{
		if (parameters is not TParams typedParams)
		{
			throw new InvalidOperationException(
				$"Task {GetType().Name} expects parameters of type {typeof(TParams).FullName}, " +
				$"but got {parameters?.GetType().FullName ?? "null"}"
			);
		}

		return ExecuteAsync(typedParams, context, ct);
	}

	protected abstract Task ExecuteAsync(TParams parameters, IPipelineContext context, CancellationToken ct);
}
