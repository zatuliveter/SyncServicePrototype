
using TaskFactory.Common;
using TaskFactory.ConsoleApp.Definitions;

namespace TaskFactory.ConsoleApp;


public class DailyProcessingPipeline : IPipeline
{
	public string Name => "DailyProcessing";

	public IReadOnlyCollection<PipelineItemBase> Items { get; } =
	[
		new PipelineItem<SendEmailTask, SendEmailParams>(
			id: "email_start",
			parameters: new SendEmailParams("test@test.com")
		),



		PipelineItem.CopyTable("load_product", parameters: ProductSync.Definition, dependsOn: []),

		//new PipelineItem<CopyData, CopyTableDefinition>("load_clients", parameters: Definitions.UsersCopyDefinition, dependsOn: []),

		new PipelineItem<CustomLoadOrdersTask>("load_orders", dependsOn: ["load_product"]),

		new PipelineItem<SendEmailTask, SendEmailParams>(
			id: "email_admin",
			parameters: new SendEmailParams("admin@test.com"),
			dependsOn: ["load_product", "load_orders"]
		)
	];
}
