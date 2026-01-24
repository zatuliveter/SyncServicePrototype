namespace TaskFactory;

public interface IPipelineRunner
{
	Task<PipelineRunResult> RunAsync(
			PipelineGroup pipelineGroup,
			RunParameters pipelineParameters,
			CancellationToken ct
		);
}
