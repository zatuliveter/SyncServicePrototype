namespace TaskFactory;

public interface IPipelineRunner
{
	Task<PipelineRunResult> RunAsync(
			Pipeline pipelineGroup,
			CancellationToken ct
		);
}
