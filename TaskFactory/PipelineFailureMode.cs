namespace TaskFactory;

public enum PipelineFailureMode
{
	/// <summary>
	/// Pipeline runner will throw exception on any error in the tasks.
	/// </summary>
	ThrowException,

	/// <summary>
	/// Pipeline will stop, but exception will not throw. Any errors can be found in PipelineRunResult. 
	/// </summary>
	FailPipeline,

	/// <summary>
	/// Just child nodes will fail, any other items continue to work.
	/// </summary>
	SkipDependentTasks
}
