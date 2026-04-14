namespace YetAnotherEcs.Utility;

/// <summary>
/// Manages a pool of type-specific IDs.
/// </summary>
/// <typeparam name="Context">An arbitrary type.</typeparam>
/// <typeparam name="T">The type to assign an ID to.</typeparam>
public static class TypedIdPool<Context, T>
{
	/// <summary>
	/// The ID associated with the type <see cref="T"/>.
	/// </summary>
	public static int Id { get; }

	static TypedIdPool()
	{
		Id = TypedIdPool<Context>.NextId++;
	}
}

internal static class TypedIdPool<Context>
{
	internal static int NextId = 0;
}
