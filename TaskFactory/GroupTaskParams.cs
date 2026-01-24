
namespace TaskFactory;

internal record GroupTaskParams(
	RunParameters RunParameters,
	IReadOnlyCollection<PipelineItemBase> Items
);