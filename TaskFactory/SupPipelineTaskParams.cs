
namespace TaskFactory;

internal record SupPipelineTaskParams(
	RunParameters RunParameters,
	IReadOnlyCollection<PipelineItemBase> Items
);