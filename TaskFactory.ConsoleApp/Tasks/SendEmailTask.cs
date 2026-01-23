namespace TaskFactory.ConsoleApp.Tasks;

public class SendEmailTask() : TaskBase<SendEmailParams>
{
	protected override async Task ExecuteAsync(SendEmailParams args, string taskId, IPipelineContext context, CancellationToken ct)
	{
		await Task.Delay(1000, ct);
		await Task.CompletedTask;
	}
}