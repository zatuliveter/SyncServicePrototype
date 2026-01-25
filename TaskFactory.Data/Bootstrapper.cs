using Microsoft.Extensions.DependencyInjection;

namespace TaskFactory.Data;

public static class Bootstrapper
{
	public static void Initialize(this IServiceCollection container)
	{
		_ = container
			.AddTransient(typeof(CopyTableTask<,>))
			.AddTransient(typeof(CopyTableChangesTask<,>));
	}
}
