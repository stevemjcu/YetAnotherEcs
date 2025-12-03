using YetAnotherEcs.General;

namespace YetAnotherEcs.Storage;

/// <summary>
/// Encapsulates the storage for all component types.
/// </summary>
internal class ComponentStore
{
	private readonly Dictionary<int, object> StoreByTypeId = [];
	private int IndexedBitmask;

	private static int Id<T>() where T : struct => TypedIdPool<ComponentStore, T>.Id;

	// TODO: Support bitmasks for more than 32 components, if necessary
	public static int Bitmask<T>() where T : struct => 1 << Id<T>();

	public void Index<T>() where T : struct
	{
		IndexedBitmask |= Bitmask<T>();
		Store<T>().Indexed = true;
	}

	public void ClearIndices(int id, int bitmask)
	{
		bitmask &= IndexedBitmask;
		for (var i = 0; bitmask > 0; i++)
		{
			var mask = 1 << i;
			if ((bitmask & mask) == 0) continue;
			Store(i).Remove(id);
			bitmask ^= mask;
		}
	}

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

	public IComponentStore Store(int id) => (IComponentStore)StoreByTypeId[id];
}

/// <summary>
/// Encapsulates the storage for one component type.
/// </summary>
/// <typeparam name="T">The component type.</typeparam>
internal class ComponentStore<T> : IComponentStore where T : struct
{
	public bool Indexed;

	private readonly Dictionary<int, T> ComponentById = [];
	private readonly Dictionary<T, HashSet<int>> IdSetByComponent = [];

	public T Get(int id) => ComponentById[id];

	public void Set(int id, T component)
	{
		if (Indexed)
		{
			if (ComponentById.ContainsKey(id)) IdSetByComponent[Get(id)].Remove(id);
			Initialize(component);
			IdSetByComponent[component].Add(id);
		}

		ComponentById[id] = component;
	}

	public void Remove(int id)
	{
		if (Indexed) IdSetByComponent[Get(id)].Remove(id);
		ComponentById.Remove(id);
	}

	#region Index

	public void Initialize(T index)
	{
		if (!IdSetByComponent.ContainsKey(index)) IdSetByComponent[index] = [];
	}

	public bool Contains(T index, int id) =>
		IdSetByComponent[index].Contains(id);

	public HashSet<int>.Enumerator GetEnumerator(T index) =>
		IdSetByComponent[index].GetEnumerator();

	#endregion
}

internal interface IComponentStore
{
	public void Remove(int id);
}