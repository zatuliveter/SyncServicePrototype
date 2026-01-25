namespace TaskFactory;

public interface IPipelineRunner
{
	Task<PipelineRunResult> RunAsync(
			Pipeline pipeline,
			CancellationToken ct
		);
}
