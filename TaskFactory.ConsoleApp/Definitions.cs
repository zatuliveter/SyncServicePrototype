namespace TaskFactory.ConsoleApp;

public static class Definitions
{
	public static readonly CopyDataDefinition UsersCopyDefinition = new() { SourceTableName = "UserTable" };
	public static readonly CopyDataDefinition ClientsCopyDefinition = new() { SourceTableName = "UserTable" };
}
