namespace YetAnotherEcs;

/// <summary>
/// An entity identifier associated with a component set.
/// </summary>
public record struct Entity(int Id, int Version, World World)
{
	internal readonly int Bitmask
	{
		get => World.Entities.GetBitmask(this);
		set
		{
			OnChange(Bitmask, value);
			World.Entities.SetBitmask(this, value);
		}
	}

	/// <summary>
	/// Set a component.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	/// <param name="component">The component.</param>
	/// <returns>This entity.</returns>
	public readonly Entity Set<T>(T component = default) where T : struct
	{
		World.Components.Set(Id, component);
		Bitmask |= 1 << World.Components.Id<T>();
		return this;
	}

	/// <summary>
	/// Remove a component.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	public readonly void Remove<T>() where T : struct
	{
		World.Components.Remove<T>(Id);
		Bitmask ^= 1 << World.Components.Id<T>();
	}

	/// <summary>
	/// Check if a component exists.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	/// <returns>True if the component exists.</returns>
	public readonly bool Contains<T>() where T : struct =>
		(Bitmask & (1 << World.Components.Id<T>())) > 0;

	/// <summary>
	/// Get a component.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	/// <returns>The component.</returns>
	public readonly T Get<T>() where T : struct => World.Components.Get<T>(Id);

	/// <summary>
	/// Get a component if it exists.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	/// <param name="component">The component.</param>
	/// <returns>True if the component exists.</returns>
	public readonly bool TryGet<T>(out T component) where T : struct
	{
		var has = Contains<T>();
		component = has ? Get<T>() : default;
		return has;
	}

	#region Filters

	internal readonly void OnChange(int bitmask1, int bitmask2)
	{
		if (bitmask1 == bitmask2) return;

		var filters = World.Filters;
		foreach (var it in filters)
		{
			if (it.Matches(bitmask1) && !it.Matches(bitmask2)) filters.RemoveEntity(it, this);
			else if (!it.Matches(bitmask1) && it.Matches(bitmask2)) filters.AddEntity(it, this);
		}
	}

	#endregion
}
