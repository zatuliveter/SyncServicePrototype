namespace TaskFactory;

public record RunParameters(
	int ParallelTaskCount,
	PipelineFailureMode FailureMode
)
{
	public static RunParameters Default => new(
		ParallelTaskCount: 100,
		FailureMode: PipelineFailureMode.FailPipeline
	);
};
