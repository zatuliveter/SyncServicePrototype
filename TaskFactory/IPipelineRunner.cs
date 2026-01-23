namespace TaskFactory;

public interface IPipelineRunner
{
	Task<PipelineRunResult> RunAsync(
			IPipeline pipeline,
			int parallelTaskCount,
			PipelineFailureMode failureMode,
			CancellationToken ct
		);
}
