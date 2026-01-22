namespace TaskFactory;

public interface IPipeline
{
	string Name { get; }
	IReadOnlyCollection<PipelineItemBase> Items { get; }
}

/// <summary>
/// Task implementations must be stateless. All state must be external.
/// </summary>
public interface ITask
{
	Task ProcessAsync(object? parameters, IPipelineContext context, CancellationToken ct);
}

public abstract class TaskBase<TParams> : ITask
{
	public Task ProcessAsync(object? parameters, IPipelineContext context, CancellationToken ct)
	{
		if (parameters is not TParams typedParams)
		{
			throw new ArgumentException($"Task expects parameters of type {typeof(TParams).Name}, but got {parameters?.GetType().Name ?? "null"}");
		}

		return ExecuteAsync(typedParams, context, ct);
	}

	protected abstract Task ExecuteAsync(TParams parameters, IPipelineContext context, CancellationToken ct);
}

public interface IPipelineContext
{
	Guid RunId { get; }

	string CurrentTaskId { get; }

	CancellationToken PipelineCancellation { get; }
}


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


public record SendEmailParams 
{

}

public class SendEmailTask() : TaskBase<SendEmailParams>
{
	protected override async Task ExecuteAsync(SendEmailParams args, IPipelineContext context, CancellationToken ct)
	{
		await Task.CompletedTask;
	}
}

public class CustomLoadOrdersTask : ITask
{
	public Task ProcessAsync(object? parameters, IPipelineContext context, CancellationToken ct)
	{
		throw new NotImplementedException();
	}
}

public record CopyDataDefinition
{
	public string SourceTableName { get; set; }
}


public static class Definitions
{
	public static CopyDataDefinition UsersCopyDefinition = new() { SourceTableName = "UserTable" };
	public static CopyDataDefinition ClientsCopyDefinition = new() { SourceTableName = "UserTable" };
}

public class CopyData : TaskBase<CopyDataDefinition>
{
	protected override Task ExecuteAsync(CopyDataDefinition args, IPipelineContext context, CancellationToken ct)
	{
		throw new NotImplementedException();
	}
}

public class DailyProcessingPipeline : IPipeline
{
	public string Name => "DailyProcessing";

	public IReadOnlyCollection<PipelineItemBase> Items { get; } =
	[
		new PipelineItem<CopyData, CopyDataDefinition>("load_users", parameters: Definitions.UsersCopyDefinition, dependsOn: []),
		new PipelineItem<CopyData, CopyDataDefinition>("load_clients", parameters: Definitions.UsersCopyDefinition, dependsOn: []),

		new PipelineItem<CustomLoadOrdersTask>("load_orders", dependsOn: ["load_users"]),

        new PipelineItem<SendEmailTask, SendEmailParams>(
			id: "email_admin",
			parameters: new SendEmailParams(),
			dependsOn: ["load_users", "load_orders"]
        )
	];
}
