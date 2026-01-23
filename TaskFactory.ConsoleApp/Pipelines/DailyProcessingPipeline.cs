using TaskFactory.Common;
using TaskFactory.ConsoleApp.Definitions;
using TaskFactory.ConsoleApp.Tasks;

namespace TaskFactory.ConsoleApp.Pipelines;


public class DailyProcessingPipeline : IPipeline
{
	public string Name => "DailyProcessing";

	public IReadOnlyCollection<PipelineItemBase> Items { get; } =
	[
		new PipelineItem<SendEmailTask, SendEmailParams>(
			id: "email_start",
			parameters: new SendEmailParams("test@test.com")
		),

		PipelineItem.CopyTable("load_products", parameters: ProductSync.Definition, dependsOn: []),

		new PipelineItem<CopyDataDemo>("load_data1", dependsOn: ["load_products", "email_start"]),
		new PipelineItem<CopyDataDemo>("load_data2", dependsOn: ["load_products"]),
		new PipelineItem<CopyDataDemo>("load_data3", dependsOn: ["load_products"]),

		new PipelineItem<CopyDataDemo>("load1", dependsOn: []),
		new PipelineItem<CopyDataDemo>("load2", dependsOn: []),
		new PipelineItem<CopyDataDemo>("load3", dependsOn: []),

		new PipelineItem<CustomLoadOrdersTask>("load_orders", dependsOn: ["load_products"]),

		new PipelineItem<SendEmailTask, SendEmailParams>(
			id: "email_admin",
			parameters: new SendEmailParams("admin@test.com"),
			dependsOn: ["load_products", "load_orders"]
		)
	];
}
