
namespace TaskFactory.ConsoleApp;

public class SendEmailTask() : TaskBase<SendEmailParams>
{
	protected override async Task ExecuteAsync(SendEmailParams args, string taskId, IPipelineContext context, CancellationToken ct)
	{
		await Task.Delay(new Random().Next(3000), ct);
		await Task.CompletedTask;
	}
}