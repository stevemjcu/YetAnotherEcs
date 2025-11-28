namespace YetAnotherEcs;

/// <summary>
/// An identifier associated with a component set.
/// </summary>
public readonly record struct Entity
{
	public readonly int Id;

	public readonly int WorldId;

	public readonly int Version;

	public Entity Set<T>(T value = default) where T : struct => this;

	public void Remove<T>() where T : struct { }

	public bool Contains<T>() where T : struct => default;

	public T Get<T>() where T : struct => default;
}
