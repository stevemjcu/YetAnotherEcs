using YetAnotherEcs.Utility;

namespace YetAnotherEcs;

/// <summary>
/// Provides static metadata for a component type.
/// </summary>
/// <typeparam name="T">The component type.</typeparam>
public static class ComponentMetadata<T> where T : struct
{
	public static int Id { get; }

	public static int Bitmask { get; }

	public static bool Indexed { get; }

	static ComponentMetadata()
	{
		var isComponent = Attribute.GetCustomAttribute(typeof(T), typeof(ComponentAttribute)) is not null;
		var isIndex = Attribute.GetCustomAttribute(typeof(T), typeof(IndexedAttribute)) is not null;

		if (!isComponent)
		{
			throw new InvalidOperationException(
				$"Cannot use the non-component type {typeof(T)} as a component.");
		}

		Id = TypedIdPool<World, T>.Id;
		Bitmask = 1 << Id;
		Indexed = isIndex;
	}
}
