namespace YetAnotherEcs;

public readonly record struct Entity(World World, int Id)
{
	public readonly Entity Set<T>(T value = default) where T : struct
	{
		World.Set(Id, value);
		return this;
	}

	public readonly Entity Remove<T>() where T : struct
	{
		World.Remove<T>(Id);
		return this;
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
