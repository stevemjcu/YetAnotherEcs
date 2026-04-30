namespace YetAnotherEcs;

/// <summary>
/// Represents a set of components associated with a unique ID.
/// </summary>
public readonly record struct Entity(World World, int Id)
{
	internal readonly int Bitmask => World.Table.GetBitmask(Id);

	/// <summary>
	/// Determines if a component exists.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	/// <returns>True if the component exists; otherwise, false.</returns>
	public readonly bool Has<T>() where T : struct
	{
		return World.Table.HasComponent<T>(Id);
	}

	/// <summary>
	/// Retrieves a component.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	/// <returns>The component value.</returns>
	public readonly T Get<T>() where T : struct
	{
		return World.Table.GetComponent<T>(Id);
	}

	/// <summary>
	/// Retrieves a component if it exists.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	/// <param name="value">The component value.</param>
	/// <returns>True if the component exists; otherwise, false.</returns>
	public readonly bool TryGet<T>(out T value) where T : struct
	{
		var exists = Has<T>();
		value = exists ? Get<T>() : default;
		return exists;
	}

	/// <summary>
	/// Overwrites a component.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	/// <param name="value">The component value.</param>
	public readonly void Set<T>(T value = default) where T : struct
	{
		var exists = Has<T>();

		if (ComponentType<T>.Indexed)
		{
			if (exists)
			{
				World.Index.OnComponentRemoved(Id, Get<T>());
			}

			World.Index.OnComponentAdded(Id, value);
		}

		World.Table.SetComponent(Id, value);

		if (!exists)
		{
			World.Index.OnStructureChanged(Id, Bitmask);
		}
	}

	/// <summary>
	/// Removes a component.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	public void Remove<T>() where T : struct
	{
		if (ComponentType<T>.Indexed)
		{
			World.Index.OnComponentRemoved(Id, Get<T>());
		}

		World.Table.RemoveComponent<T>(Id);
		World.Index.OnStructureChanged(Id, Bitmask);
	}
}
