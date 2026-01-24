namespace TaskFactory;

public interface IPipelineRunner
{
	Task<PipelineRunResult> RunAsync(
			IPipeline pipeline,
			RunParameters pipelineParameters,
			CancellationToken ct
		);
}
