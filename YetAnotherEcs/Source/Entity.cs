namespace YetAnotherEcs;

public readonly record struct Entity(int Id, int WorldId)
{
	private readonly World World => World.WorldById[WorldId]!;

	public readonly void Set<T>(T value = default) where T : struct
	{
		World.Set(Id, value);
	}

	public readonly void Remove<T>() where T : struct
	{
		World.Remove<T>(Id);
	}

	public readonly bool Has<T>() where T : struct
	{
		return World.Has<T>(Id);
	}

	public readonly T Get<T>() where T : struct
	{
		return World.Get<T>(Id);
	}
}
