using YetAnotherEcs.General;

namespace YetAnotherEcs;

public static class Component<T> where T : struct
{
	public static readonly int Id;

	public static readonly int Bitmask;

	public static readonly bool Indexed;

	static Component()
	{
		Id = TypedIdPool<World, T>.Id;
		Bitmask = 1 << Id;
		Indexed = Attribute.GetCustomAttribute(typeof(T), typeof(IndexedAttribute)) is not null;
	}
}
