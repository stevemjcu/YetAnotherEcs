namespace YetAnotherEcs;

/// <summary>
/// A filter signature associated with an entity set.
/// </summary>
public record struct Filter(World World)
{
	private int IncludeBitmask;
	private int ExcludeBitmask;

	/// <summary>
	/// Include a component type on the filter.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	/// <returns>This filter.</returns>
	public Filter Include<T>() where T : struct
	{
		IncludeBitmask |= 1 << World.Components.GetTypeId<T>();
		return this;
	}

	/// <summary>
	/// Exclude a component type on the filter.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	/// <returns>This filter.</returns>
	public Filter Exclude<T>() where T : struct
	{
		ExcludeBitmask |= 1 << World.Components.GetTypeId<T>();
		return this;
	}

	/// <summary>
	/// Register filter for automatic updates.
	/// </summary>
	/// <returns>This filter.</returns>
	public readonly Filter Build()
	{
		if (World.Filters.Contains(this)) return this;
		World.Filters.Add(this);

		foreach (var it in World.Entities)
		{
			World.Filters.Evaluate(it.Id, it.Bitmask);
		}

		return this;
	}

	/// <summary>
	/// Get the entity ID set associated with a filter.
	/// </summary>
	/// <returns>The entity set.</returns>
	public readonly IReadOnlySet<int> Query() => World.Filters.Query(this);

	internal readonly bool Matches(int bitmask)
	{
		return
			(bitmask & IncludeBitmask) == IncludeBitmask &&
			(bitmask & ExcludeBitmask) == 0;
	}
}
