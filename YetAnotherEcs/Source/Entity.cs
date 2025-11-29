namespace YetAnotherEcs;

/// <summary>
/// An identifier associated with a component set.
/// </summary>
public readonly record struct Entity(int Id, int Version, World World)
{
	internal int Bitmask
	{
		get => World.BitmaskByEntityId[Id];
		set => World.BitmaskByEntityId[Id] = value;
	}

	/// <summary>
	/// Copy this entity's components.
	/// </summary>
	/// <returns></returns>
	public Entity Copy() => World.Copy(this);

	/// <summary>
	/// Destroy this entity.
	/// </summary>
	/// <param name="entity">The entity.</param>
	public void Destroy() => World.Destroy(this);

	/// <summary>
	/// Set a component.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	/// <param name="component">The component.</param>
	/// <returns>This entity.</returns>
	public Entity Set<T>(T component = default) where T : struct, IComponent<T>
	{
		World.Components.Set(this, component);
		return this;
	}

	/// <summary>
	/// Remove a component.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	public void Remove<T>() where T : struct, IComponent<T> =>
		World.Components.Remove<T>(this);

	/// <summary>
	/// Check if a component exists.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	/// <returns>True if the component exists; otherwise, false.</returns>
	public bool Has<T>() where T : struct, IComponent<T> =>
		World.Components.Has<T>(this);

	/// <summary>
	/// Get a component.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	/// <returns>The component.</returns>
	public T Get<T>() where T : struct, IComponent<T> =>
		World.Components.Get<T>(this);
}
