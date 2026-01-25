
namespace TaskFactory;

internal class SubPipelineTask
	(
		IPipelineRunner pipelineRunner
	)
	: TaskBase<SubPipelineTaskParams>
{
	protected override async Task ExecuteAsync(
		SubPipelineTaskParams parameters,
		string taskId,
		IPipelineContext context,
		CancellationToken ct
	)
	{
		Pipeline subPipeline = new(
			id: $"{context.PipelineName}.{taskId}",
			runParams: parameters.RunParameters,
			items: parameters.Items
		);

		PipelineRunResult result = await pipelineRunner.RunAsync(subPipeline, ct);

		result.ThrowOnError();
	}
}
