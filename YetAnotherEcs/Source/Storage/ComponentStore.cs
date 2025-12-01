using YetAnotherEcs.General;

namespace YetAnotherEcs.Storage;

/// <summary>
/// Encapsulates the storage for all components.
/// </summary>
internal class ComponentStore
{
	private readonly Dictionary<int, object> StoreByTypeId = [];

	private static int Id<T>() where T : struct => TypedIdPool<ComponentStore, T>.Id;

	// FIXME: Support bitmasks for more than 32 components, if necessary
	public static int Bitmask<T>() where T : struct => 1 << Id<T>();

	public void Set<T>(int id, T component) where T : struct => GetStore<T>()[id] = component;

	public void Remove<T>(int id) where T : struct => GetStore<T>().Remove(id);

	public T Get<T>(int id) where T : struct => GetStore<T>()[id];

	private Dictionary<int, T> GetStore<T>() where T : struct
	{
		var id = Id<T>();

		if (!StoreByTypeId.TryGetValue(id, out var value))
		{
			value = new Dictionary<int, T>();
			StoreByTypeId.Add(id, value);
		}

		return (Dictionary<int, T>)value;
	}
}