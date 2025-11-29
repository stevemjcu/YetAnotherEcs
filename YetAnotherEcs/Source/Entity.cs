namespace YetAnotherEcs;

/// <summary>
/// An identifier associated with a component set.
/// </summary>
public readonly record struct Entity(int id, int worldId, int version)
{
	public readonly int Id = id;

	public readonly int WorldId = worldId;

	public readonly int Version = version;

	private World World => World.WorldById[WorldId];

	/// <summary>
	/// Set a component.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	/// <param name="component">The component.</param>
	/// <returns>This entity.</returns>
	public Entity Set<T>(T component = default) where T : struct, IComponent<T> =>
		World.Set(this, component);

	public void Remove<T>() where T : struct, IComponent<T> { }

	public bool Contains<T>() where T : struct, IComponent<T> => default;

	public T Get<T>() where T : struct, IComponent<T> => default;
}
