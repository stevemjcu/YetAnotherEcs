namespace YetAnotherEcs.General;

/// <summary>
/// An ID pool which assigns a 0-indexed, incrementing static ID to the given type when the static 
/// constructor for <typeparamref name="T"/>, <typeparamref name="V"/> is called.
/// </summary>
/// <typeparam name="T">The context which shares an ID pool.</typeparam>
/// <typeparam name="V">The type to assign an ID to.</typeparam>
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
