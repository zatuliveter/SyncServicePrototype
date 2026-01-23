
namespace TaskFactory.ConsoleApp;

public class SendEmailTask() : TaskBase<SendEmailParams>
{
	protected override async Task ExecuteAsync(SendEmailParams args, IPipelineContext context, CancellationToken ct)
	{
		await Task.CompletedTask;
	}
}