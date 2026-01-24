namespace TaskFactory;


public sealed class PipelineValidator : IPipelineValidator
{
	public void Validate(Pipeline pipeline)
	{
		if (pipeline.Items.Count == 0)
			throw new InvalidOperationException("Pipeline contains no items.");

		// 1. Unique IDs
		string[] duplicates = pipeline.Items
			.GroupBy(x => x.Id, StringComparer.OrdinalIgnoreCase)
			.Where(g => g.Count() > 1)
			.Select(g => g.Key)
			.ToArray();

		if (duplicates.Length > 0)
			throw new InvalidOperationException(
				"Duplicate task ids: " + string.Join(", ", duplicates)
			);

		Dictionary<string, PipelineItemBase> itemsById = pipeline.Items.ToDictionary(x => x.Id, StringComparer.OrdinalIgnoreCase);

		// 2. All dependencies must exist
		foreach (PipelineItemBase item in pipeline.Items)
		{
			foreach (string dep in item.DependsOn)
			{
				if (!itemsById.ContainsKey(dep))
				{
					throw new InvalidOperationException(
						$"Task '{item.Id}' depends on missing task '{dep}'."
					);
				}
			}
		}

		// 3. Cycle detection (DFS)
		HashSet<string> visited = new(StringComparer.OrdinalIgnoreCase);
		HashSet<string> stack = new(StringComparer.OrdinalIgnoreCase);

		foreach (PipelineItemBase item in pipeline.Items)
		{
			Visit(item.Id);
		}

		void Visit(string id)
		{
			if (stack.Contains(id))
				throw new InvalidOperationException($"Cycle detected at task '{id}'.");

			if (visited.Contains(id))
				return;

			visited.Add(id);
			stack.Add(id);

			PipelineItemBase node = itemsById[id];
			foreach (string dep in node.DependsOn)
			{
				Visit(dep);
			}

			stack.Remove(id);
		}
	}
}
