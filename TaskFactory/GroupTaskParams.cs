
namespace TaskFactory;

public record GroupTaskParams(
	IReadOnlyCollection<PipelineItemBase> Items,
	RunParameters PipelineParameters
);