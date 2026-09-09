namespace YetAnotherEcs;

/// <summary>
/// A unique identifier associated with a set of components.
/// </summary>
public readonly record struct Entity(World World, int Id)
{
	/// <summary>
	/// Determines if a component exists.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	/// <returns>True if the component exists; otherwise, false.</returns>
	public readonly bool Has<T>() where T : struct
	{
		return World.Has<T>(Id);
	}

	/// <summary>
	/// Retrieves a component.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	/// <returns>The component value.</returns>
	public readonly T Get<T>() where T : struct
	{
		return World.Get<T>(Id);
	}

	/// <summary>
	/// Retrieves a component if it exists.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	/// <param name="value">The component value.</param>
	/// <returns>True if the component exists; otherwise, false.</returns>
	public readonly bool TryGet<T>(out T value) where T : struct
	{
		return World.TryGet(Id, out value);
	}

	/// <summary>
	/// Overwrites a component.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	/// <param name="value">The component value.</param>
	public readonly void Set<T>(T value = default) where T : struct
	{
		World.Set<T>(Id, value);
	}

	/// <summary>
	/// Removes a component.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	public void Remove<T>() where T : struct
	{
		World.Remove<T>(Id);
	}
}
