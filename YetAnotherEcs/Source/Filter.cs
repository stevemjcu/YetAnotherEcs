using YetAnotherEcs.Storage;

namespace YetAnotherEcs;

/// <summary>
/// A filter signature associated with an entity set.
/// </summary>
public record struct Filter(World World)
{
	private int IncludeBitmask;
	private int ExcludeBitmask;

	/// <summary>
	/// Include a component type.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	/// <returns>This filter.</returns>
	public Filter Include<T>() where T : struct
	{
		IncludeBitmask |= ComponentStore.Bitmask<T>();
		return this;
	}

	/// <summary>
	/// Exclude a component type.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	/// <returns>This filter.</returns>
	public Filter Exclude<T>() where T : struct
	{
		ExcludeBitmask |= ComponentStore.Bitmask<T>();
		return this;
	}

	/// <summary>
	/// Populate and register for automatic updates.
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
	/// Get the associated entity ID set.
	/// </summary>
	/// <returns>The entity ID set.</returns>
	// TODO: Can expose enumerator and contains method rather than reveal implementation
	public readonly IReadOnlySet<int> AsSet() => World.Filters.AsSet(this);

	internal readonly bool Matches(int bitmask)
	{
		return
			(bitmask & IncludeBitmask) == IncludeBitmask &&
			(bitmask & ExcludeBitmask) == 0;
	}
}
