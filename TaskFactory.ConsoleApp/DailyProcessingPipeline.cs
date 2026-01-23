
namespace TaskFactory.ConsoleApp;


public class DailyProcessingPipeline : IPipeline
{
	public string Name => "DailyProcessing";

	public IReadOnlyCollection<PipelineItemBase> Items { get; } =
	[
		new PipelineItem<CopyData, CopyDataDefinition>("load_users", parameters: Definitions.UsersCopyDefinition, dependsOn: []),
	new PipelineItem<CopyData, CopyDataDefinition>("load_clients", parameters: Definitions.UsersCopyDefinition, dependsOn: []),

	new PipelineItem<CustomLoadOrdersTask>("load_orders", dependsOn: ["load_users"]),

	new PipelineItem<SendEmailTask, SendEmailParams>(
		id: "email_admin",
		parameters: new SendEmailParams(),
		dependsOn: ["load_users", "load_orders"]
	)
	];
}
