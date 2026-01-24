
namespace TaskFactory;

internal record PipelineTaskParams(
	RunParameters RunParameters,
	IReadOnlyCollection<PipelineItemBase> Items
);