using YetAnotherEcs.Utility;

namespace YetAnotherEcs;

/// <summary>
/// Provides static metadata for a component type.
/// </summary>
/// <typeparam name="T">The component type.</typeparam>
public static class ComponentType<T> where T : struct
{
	public static int Id { get; }

	public static int Bitmask { get; }

	static ComponentType()
	{
		Id = TypedIdPool<World, T>.Id;
		Bitmask = 1 << Id;
	}
}
