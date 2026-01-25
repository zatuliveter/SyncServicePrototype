namespace TaskFactory.Common;

public interface ISyncState
{
	Task SaveAsync(object value);
	Task<object?> GetAsync();
}