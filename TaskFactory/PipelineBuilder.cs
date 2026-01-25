
namespace TaskFactory;

public sealed class PipelineBuilder
{
	private readonly string _id;
	private readonly List<PipelineItem> _items = [];
	private readonly string[] _dependsOn;
	private readonly RunParameters _runParams;

	internal PipelineBuilder(
		string id,
		string[]? dependsOn = null,
		RunParameters? runParams = null
	)
	{
		_id = id;
		_runParams = runParams ?? RunParameters.Default;
		_dependsOn = dependsOn ?? [];
	}

	public PipelineBuilder AddTask<TTask, TParams>(
		string id,
		TParams parameters,
		params string[]? dependsOn
	)
		where TTask : TaskBase<TParams>
	{
		_items.Add(new PipelineItem(id, typeof(TTask), dependsOn, parameters));
		return this;
	}

	public PipelineBuilder AddTask<TTask>(
		string id,
		params string[]? dependsOn
	)
		where TTask : ITask
	{
		_items.Add(new PipelineItem(id, typeof(TTask), dependsOn, null));
		return this;
	}

	public PipelineBuilder AddPipeline(
		string id,
		Action<PipelineBuilder> configure
	)
	{
		var builder = new PipelineBuilder(id, null, null);
		configure(builder);
		_items.Add(builder.Build());
		return this;
	}

	public PipelineBuilder AddPipeline(
		string id,
		string[] dependsOn,
		RunParameters runParams,
		Action<PipelineBuilder> configure
	)
	{
		var builder = new PipelineBuilder(id, dependsOn, runParams);
		configure(builder);
		_items.Add(builder.Build());
		return this;
	}

	public PipelineBuilder AddPipeline(
		string id,
		string[] dependsOn,
		Action<PipelineBuilder> configure
	)
	{
		var builder = new PipelineBuilder(id, dependsOn, null);
		configure(builder);
		_items.Add(builder.Build());
		return this;
	}

	public PipelineBuilder AddPipeline(
		string id,
		RunParameters runParams,
		Action<PipelineBuilder> configure
	)
	{
		var builder = new PipelineBuilder(id, null, runParams);
		configure(builder);
		_items.Add(builder.Build());
		return this;
	}

	internal Pipeline Build()
	{
		return new Pipeline(
			id: _id,
			dependsOn: _dependsOn,
			runParams: _runParams,
			items: _items
		);
	}
}

public static class PipelineFactory
{
	public static Pipeline CreatePipeline(
		string id,
		RunParameters runParams,
		Action<PipelineBuilder> configure
	)
	{
		var builder = new PipelineBuilder(id, dependsOn: null, runParams);
		configure(builder);
		return builder.Build();
	}
}
