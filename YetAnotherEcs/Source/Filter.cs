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
		IncludeBitmask |= 1 << World.Components.Id<T>();
		return this;
	}

	/// <summary>
	/// Exclude a component type on the filter.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	/// <returns>This filter.</returns>
	public Filter Exclude<T>() where T : struct
	{
		ExcludeBitmask |= 1 << World.Components.Id<T>();
		return this;
	}

	/// <summary>
	/// Register filter for automatic updates.
	/// </summary>
	/// <returns>This filter.</returns>
	public Filter Register()
	{
		if (World.Filters.Contains(this)) return this;
		World.Filters.Add(this);

		foreach (var it in World.Entities)
		{
			if (!Matches(it.Bitmask)) continue;
			World.Filters.AddEntity(this, it);
		}

		return this;
	}

	internal bool Matches(int bitmask)
	{
		return
			(bitmask & IncludeBitmask) == IncludeBitmask &&
			(bitmask & ExcludeBitmask) == 0;
	}
}
