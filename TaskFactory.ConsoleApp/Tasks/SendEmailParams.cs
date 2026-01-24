namespace TaskFactory.ConsoleApp.Tasks;

public record SendEmailParams
{
	public string[] Emails { get; init; }

	public SendEmailParams(string email, params string[] emails)
	{
		if (string.IsNullOrWhiteSpace(email))
		{
			throw new ArgumentException("The first email cannot be empty.", nameof(email));
		}

		Emails = [email, .. emails];
	}
}