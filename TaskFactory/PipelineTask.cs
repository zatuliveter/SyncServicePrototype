
namespace TaskFactory;

internal class PipelineTask
	(
		IPipelineRunner pipelineRunner
	)
	: TaskBase<GroupTaskParams>
{
	protected override Task ExecuteAsync(GroupTaskParams parameters, string taskId, IPipelineContext context, CancellationToken ct)
	{
		Pipeline pipeline = new(
			id: $"{context.PipelineName}.{taskId}",
			runParams: parameters.RunParameters,
			items: parameters.Items
		);
		return pipelineRunner.RunAsync(pipeline, ct);
	}
}
