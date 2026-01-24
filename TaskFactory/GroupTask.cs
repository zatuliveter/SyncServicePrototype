
namespace TaskFactory;

internal class GroupTask 
	(
		IPipelineRunner pipelineRunner
	)
	: TaskBase<GroupTaskParams>
{
	protected override Task ExecuteAsync(GroupTaskParams parameters, string taskId, IPipelineContext context, CancellationToken ct)
	{
		Pipeline subPipeline = new(){ Name = $"{context.PipelineName}.{taskId}", Items = parameters.Items };
		return pipelineRunner.RunAsync(subPipeline, parameters.PipelineParameters,ct);
	}
}
