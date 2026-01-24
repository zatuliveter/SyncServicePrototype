using TaskFactory.Common;
using TaskFactory.ConsoleApp.Definitions;
using TaskFactory.ConsoleApp.Tasks;

namespace TaskFactory.ConsoleApp.Pipelines;

public static class Pipelines 
{
	public static readonly PipelineGroup DailyProcessing = new(	
		id: "DailyProcessing",
		runParams: new RunParameters(ParallelTaskCount: 10, FailureMode: PipelineFailureMode.FailPipeline),
		items: [
			new PipelineItem<SendEmailTask, SendEmailParams>(
				id: "email_start",
				parameters: new SendEmailParams("test@test.com")
			),

			PipelineItem.CopyTable("load_products", parameters: ProductSync.Definition),

			new PipelineGroup(
				id: "group1", 
				dependsOn: ["load_products", "email_start"], 
				runParams: new RunParameters(ParallelTaskCount: 3, FailureMode: PipelineFailureMode.FailPipeline), 
				items: [
					new PipelineItem<CopyDataDemo>("data1"),
					new PipelineItem<CopyDataDemo>("data2"),
					new PipelineItem<CopyDataDemo>("data3", dependsOn: ["data1"]),
				]
			),

			new PipelineGroup(id: "group2", dependsOn: ["email_start"], items: [
				new PipelineItem<CopyDataDemo>("load1"),
				new PipelineItem<CopyDataDemo>("load2"),
				new PipelineItem<CopyDataDemo>("load3")
			]),

			new PipelineItem<CustomLoadOrdersTask>("load_orders", dependsOn: ["load_products"]),

			new PipelineItem<SendEmailTask, SendEmailParams>(
				id: "email_admin",
				parameters: new SendEmailParams("admin@test.com"),
				dependsOn: ["load_products", "load_orders"]
			)
		]
	);
}
