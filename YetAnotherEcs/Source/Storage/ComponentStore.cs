using YetAnotherEcs.General;

namespace YetAnotherEcs.Storage;

/// <summary>
/// Encapsulates the storage for all component types (up to 32).
/// </summary>
internal class ComponentStore
{
	private readonly Dictionary<int, object> StoreByTypeId = [];
	private int IndexBitmask;

	private static int Id<T>() where T : struct => TypedIdPool<ComponentStore, T>.Id;

	public static int Bitmask<T>() where T : struct => 1 << Id<T>();

	public void Index<T>() where T : struct
	{
		IndexBitmask |= Bitmask<T>();
		Store<T>().Indexed = true;
	}

	public void ClearIndices(int id, int bitmask)
	{
		bitmask &= IndexBitmask;
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
	private readonly Dictionary<T, HashSet<int>> IdSetByIndex = []; // indexed
	private static readonly HashSet<int> Empty = [];

	public T this[int id]
	{
		get => ComponentById[id];
		set
		{
			if (Indexed)
			{
				RemoveIndex(id);
				AddIndex(value, id);
			}

			ComponentById[id] = value;
		}
	}

	public void Remove(int id)
	{
		if (Indexed) RemoveIndex(id);
		ComponentById.Remove(id);
	}

	#region Index

	public bool Contains(T index, int id) =>
		IdSetByIndex.TryGetValue(index, out var set) && set.Contains(id);

	public HashSet<int>.Enumerator GetEnumerator(T index) =>
		IdSetByIndex.TryGetValue(index, out var set) ? set.GetEnumerator() : Empty.GetEnumerator();

	private void AddIndex(T index, int id)
	{
		if (IdSetByIndex.TryGetValue(index, out var set)) set.Add(id);
		else IdSetByIndex[index] = [id];
	}

	private void RemoveIndex(int id)
	{
		if (!ComponentById.ContainsKey(id)) return;

		var index = this[id];
		var set = IdSetByIndex[index];

		set.Remove(id);
		if (set.Count == 0) IdSetByIndex.Remove(index);
	}

	#endregion
}

internal interface IComponentStore
{
	public void Remove(int id);
}