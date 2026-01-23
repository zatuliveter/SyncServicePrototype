namespace TaskFactory.ConsoleApp.Tasks;

public record SendEmailParams
{
	public string[] Emails { get; init; }

	public SendEmailParams(string firstEmail, params string[] otherEmails)
	{
		if (string.IsNullOrWhiteSpace(firstEmail))
		{
			throw new ArgumentException("The first email cannot be empty.", nameof(firstEmail));
		}

		Emails = [firstEmail, .. otherEmails];
	}
}