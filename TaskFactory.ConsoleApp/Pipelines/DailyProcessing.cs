using TaskFactory.Data;
using TaskFactory.ConsoleApp.Definitions;
using TaskFactory.ConsoleApp.Tasks;

namespace TaskFactory.ConsoleApp.Pipelines;

public static class Pipelines
{
	public static readonly Pipeline DailyProcessing = PipelineFactory.CreatePipeline(
		id: "daily",
		runParams: new RunParameters(ParallelTaskCount: 1, FailureMode: PipelineFailureMode.ThrowException),
		p =>
		{

			p.AddTask<SendEmailTask, SendEmailParams>(
				id: "email_start",
				parameters: new SendEmailParams("test@test.com")
			);

			p.AddCopyTableTask("load_products", ProductSync.Definition);

			p.AddPipeline(
				id: "group1",
				dependsOn: ["load_products", "email_start"],
				runParams: new RunParameters(3, PipelineFailureMode.FailPipeline),
				g =>
				{
					g.AddTask<CopyDataDemo>("data1");
					g.AddTask<CopyDataDemo>("data2");
					g.AddTask<CopyDataDemo>("data3", "data1");
				}
			);

			p.AddPipeline(
				id: "group2",
				dependsOn: ["email_start"],
				g =>
				{
					g.AddTask<CopyDataDemo>("load1");
					g.AddTask<CopyDataDemo>("load2");
					g.AddTask<CopyDataDemo>("load3");
					g.AddTask<CopyDataDemo>("load4");
					g.AddTask<CopyDataDemo>("load5");
					g.AddTask<CopyDataDemo>("load6");
					g.AddTask<CopyDataDemo>("load7");
					g.AddTask<CopyDataDemo>("load8");
					g.AddTask<CopyDataDemo>("load9");
				}
			);

			p.AddTask<CustomLoadOrdersTask>(id: "load_orders", dependsOn: "load_products");

			p.AddTask<SendEmailTask, SendEmailParams>(
				id: "email_admin",
				dependsOn: ["load_products", "load_orders"],
				parameters: new SendEmailParams("admin@test.com")
			);
		});
}
