using YetAnotherEcs.General;

namespace YetAnotherEcs;

public static class Component<T> where T : struct
{
	public static readonly int Id;

	public static readonly int Bitmask;

	public static readonly bool Indexed;

	static Component()
	{
		var isComponent = Attribute.GetCustomAttribute(typeof(T), typeof(ComponentAttribute)) is not null;
		var isIndex = Attribute.GetCustomAttribute(typeof(T), typeof(IndexedComponentAttribute)) is not null;

		if (!isComponent && !isIndex)
		{
			throw new InvalidOperationException($"Cannot use the non-component type {typeof(T)} as a component.");
		}

		Id = TypedIdPool<World, T>.Id;
		Bitmask = 1 << Id;
		Indexed = isIndex;
	}
}
