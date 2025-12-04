namespace YetAnotherEcs.General;

public static class TypedIdPool<T, V>
{
	public readonly static int Id;

	static TypedIdPool()
	{
		Id = TypedIdPool<T>.NextId++;
	}
}

internal static class TypedIdPool<T>
{
	internal static int NextId = 0;
}
