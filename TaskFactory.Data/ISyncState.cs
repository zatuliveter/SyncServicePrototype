namespace TaskFactory.Data;

public interface ISyncState
{
	Task SaveAsync(object value);
	Task<object?> GetAsync();
}