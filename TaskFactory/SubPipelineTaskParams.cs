
namespace TaskFactory;

internal record SubPipelineTaskParams(
	RunParameters RunParameters,
	IReadOnlyCollection<PipelineItem> Items
);