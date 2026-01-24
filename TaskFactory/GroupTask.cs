
namespace TaskFactory;

internal class GroupTask 
	(
		IPipelineRunner pipelineRunner
	)
	: TaskBase<GroupTaskParams>
{
	protected override Task ExecuteAsync(GroupTaskParams parameters, string taskId, IPipelineContext context, CancellationToken ct)
	{
		PipelineGroup subPipeline = new(id: $"{context.PipelineName}.{taskId}", items: parameters.Items);
		return pipelineRunner.RunAsync(subPipeline, parameters.PipelineParameters,ct);
	}
}
