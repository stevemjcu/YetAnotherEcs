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
	/// Returns true if the ID exists in this entity set.
	/// </summary>
	/// <param name="id">The entity ID.</param>
	/// <returns>True if this set contains the entity ID.</returns>
	public readonly bool Contains(int id) => Matches(World.Entities.GetBitmask(id));

	/// <summary>
	/// Enumerates this entity set by ID.
	/// </summary>
	public readonly HashSet<int>.Enumerator GetEnumerator() => World.Filters.GetEnumerator(this);

	internal readonly bool Matches(int bitmask)
	{
		return
			(bitmask & IncludeBitmask) == IncludeBitmask &&
			(bitmask & ExcludeBitmask) == 0;
	}
}
