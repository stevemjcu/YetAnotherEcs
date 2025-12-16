namespace YetAnotherEcs;

public readonly record struct Entity(World World, int Id)
{
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

	public readonly bool TryGet<T>(out T value) where T : struct
	{
		var has = World.Has<T>(Id);
		value = has ? World.Get<T>(Id) : default;
		return has;
	}

	public readonly T Get<T>() where T : struct
	{
		return World.Get<T>(Id);
	}
}
