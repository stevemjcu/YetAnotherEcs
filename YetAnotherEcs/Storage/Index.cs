using System.Collections;
using YetAnotherEcs.Utility;

namespace YetAnotherEcs.Storage;

internal class Index
{
	private static readonly SparseSet EmptySet = [];
	private readonly Dictionary<Filter, SparseSet> EntityIdSetByFilter = [];
	private readonly Dictionary<int, object> IndexStoreByTypeId = [];

	public void RegisterFilter(Filter filter)
	{
		EntityIdSetByFilter[filter] = [];
	}

	public void RegisterComponentType<T>() where T : struct
	{
		IndexStoreByTypeId.Add(ComponentType<T>.Id, new Dictionary<T, SparseSet>());
	}

	public bool HasFilter(Filter filter)
	{
		return EntityIdSetByFilter.ContainsKey(filter);
	}

	public bool HasComponentType<T>() where T : struct
	{
		return IndexStoreByTypeId.ContainsKey(ComponentType<T>.Id);
	}

	public SparseSet GetEntities(Filter filter)
	{
		return EntityIdSetByFilter[filter];
	}

	public SparseSet GetEntities<T>(T value) where T : struct
	{
		return GetIndexStore<T>().TryGetValue(value, out var set) ? set : EmptySet;
	}

	public void OnStructureChanged(int id, int bitmask)
	{
		foreach (var it in EntityIdSetByFilter)
		{
			if (it.Key.Matches(bitmask))
			{
				it.Value.Add(id);
			}
			else
			{
				it.Value.Remove(id);
			}
		}
	}

	public void OnComponentAdded<T>(int id, T index) where T : struct
	{
		var store = GetIndexStore<T>();

		if (!store.TryGetValue(index, out var set))
		{
			store[index] = set = [];
		}

		set.Add(id);
	}

	public void OnComponentRemoved<T>(int id, T index) where T : struct
	{
		var store = GetIndexStore<T>();

		if (store.TryGetValue(index, out var set))
		{
			set.Remove(id);
		}
	}

	public void OnEntityDeleted(int id)
	{
		foreach (var it in EntityIdSetByFilter.Values)
		{
			it.Remove(id);
		}

		foreach (var store in IndexStoreByTypeId.Values.Cast<IDictionary>())
		{
			foreach (SparseSet it in store.Values)
			{
				it.Remove(id);
			}
		}
	}

	private Dictionary<T, SparseSet> GetIndexStore<T>() where T : struct
	{
		var typeId = ComponentType<T>.Id;

		if (!IndexStoreByTypeId.TryGetValue(typeId, out var value))
		{
			value = new Dictionary<T, SparseSet>();
			IndexStoreByTypeId.Add(typeId, value);
		}

		// Maps entity ID set by component
		return (Dictionary<T, SparseSet>)value;
	}
}
