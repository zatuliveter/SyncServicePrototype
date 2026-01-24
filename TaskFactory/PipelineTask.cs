
namespace TaskFactory;

internal class PipelineTask
	(
		IPipelineRunner pipelineRunner
	)
	: TaskBase<PipelineTaskParams>
{
	protected override Task ExecuteAsync(PipelineTaskParams parameters, string taskId, IPipelineContext context, CancellationToken ct)
	{
		Pipeline pipeline = new(
			id: $"{context.PipelineName}.{taskId}",
			runParams: parameters.RunParameters,
			items: parameters.Items
		);
		return pipelineRunner.RunAsync(pipeline, ct);
	}
}
