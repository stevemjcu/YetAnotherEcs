using YetAnotherEcs.General;

namespace YetAnotherEcs.Storage;

/// <summary>
/// Encapsulates the storage for all component types.
/// </summary>
internal class ComponentStore
{
	private readonly Dictionary<int, object> StoreByTypeId = [];

	private static int Id<T>() where T : struct => TypedIdPool<ComponentStore, T>.Id;

	// TODO: Support bitmasks for more than 32 components, if necessary
	public static int Bitmask<T>() where T : struct => 1 << Id<T>();

	public ComponentStore<T> Store<T>() where T : struct
	{
		var id = Id<T>();

		if (!StoreByTypeId.TryGetValue(id, out var value))
		{
			value = new ComponentStore<T>();
			StoreByTypeId.Add(id, value);
		}

		return (ComponentStore<T>)value;
	}
}

/// <summary>
/// Encapsulates the storage for one component type.
/// </summary>
/// <typeparam name="T">The component type.</typeparam>
internal class ComponentStore<T> where T : struct
{
	public bool Indexed = false;

	private readonly Dictionary<int, T> ComponentById = [];
	private readonly Dictionary<T, HashSet<int>> IdSetByComponent = [];

	public void Set(int id, T component)
	{
		if (Indexed)
		{
			IdSetByComponent[Get(id)].Remove(id);
			IdSetByComponent[component].Add(id);
		}

		ComponentById[id] = component;
	}

	public void Remove(int id)
	{
		if (Indexed) IdSetByComponent[Get(id)].Remove(id);
		ComponentById.Remove(id);
	}

	public T Get(int id) => ComponentById[id];

	public bool Contains(T index, int id) =>
		IdSetByComponent[index].Contains(id);

	public HashSet<int>.Enumerator GetEnumerator(T index) =>
		IdSetByComponent[index].GetEnumerator();
}