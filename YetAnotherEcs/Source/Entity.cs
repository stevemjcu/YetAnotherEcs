namespace YetAnotherEcs;

public readonly record struct Entity(int Id, int VersionId, int WorldId)
{
	private World World => World.WorldById[WorldId]!;

	internal int Bitmask => World.Registry.BitmaskById[Id];

	public void Set<T>(T value) where T : struct
	{
		World.Registry.Set(Id, value);
	}

	public void Remove<T>() where T : struct
	{
		World.Registry.Remove<T>(Id);
	}

	public bool Has<T>() where T : struct
	{
		return World.Registry.Has<T>(Id);
	}

	public T Get<T>() where T : struct
	{
		return World.Registry.Get<T>(Id);
	}

	public bool TryGet<T>(out T value) where T : struct
	{
		return World.Registry.TryGet<T>(Id, out value);
	}
}
