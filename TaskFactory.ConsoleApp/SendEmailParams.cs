
namespace TaskFactory.ConsoleApp;

public record SendEmailParams
{
	public string[] Emails { get; set; } = [];
}
