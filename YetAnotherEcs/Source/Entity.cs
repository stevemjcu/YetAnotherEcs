namespace YetAnotherEcs;

/// <summary>
/// An identifier associated with a component set.
/// </summary>
public readonly record struct Entity(int id, int worldId, int version)
{
	public readonly int Id = id;

	public readonly int WorldId = worldId;

	public readonly int Version = version;

	public Entity Set<T>(T value = default) where T : struct => this;

	public void Remove<T>() where T : struct { }

	public bool Contains<T>() where T : struct => default;

	public T Get<T>() where T : struct => default;
}
