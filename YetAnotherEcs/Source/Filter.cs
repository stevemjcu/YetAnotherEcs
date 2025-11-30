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
	public void Register()
	{
		if (World.Filters.Contains(this)) return;

		var set = new HashSet<Entity>();
		foreach (var it in World.Entities) if (Check(it.Bitmask)) set.Add(it);
		World.Filters.Add(this, set);
	}

	internal bool Check(int bitmask)
	{
		return
			(bitmask & IncludeBitmask) == IncludeBitmask &&
			(bitmask & ExcludeBitmask) == 0;
	}
}
