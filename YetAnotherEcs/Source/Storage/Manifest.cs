using System.Collections;
using YetAnotherEcs.General;

namespace YetAnotherEcs.Storage;

internal class Manifest
{
	private readonly Dictionary<Filter, SparseSet> SetByFilter = [];
	private readonly Dictionary<int, object> IndexStoreByType = [];

	// Indexes entity ID set by component
	private Dictionary<T, SparseSet> GetIndexStore<T>() where T : struct, IComponent
	{
		var typeId = IComponent.GetId<T>();

		if (!IndexStoreByType.TryGetValue(typeId, out var value))
		{
			value = new Dictionary<int, T>();
			IndexStoreByType.Add(typeId, value);
		}

		return (Dictionary<T, SparseSet>)value;
	}

	public void OnStructureChanged(int id, int bitmask)
	{
		foreach (var it in SetByFilter)
		{
			if (it.Key.Compare(bitmask))
			{
				it.Value.Add(id);
			}
			else
			{
				it.Value.Remove(id);
			}
		}
	}

	public void OnIndexAdded<T>(int id, T index) where T : struct, IComponent
	{
		var store = GetIndexStore<T>();

		if (!store.TryGetValue(index, out var set))
		{
			store[index] = set = [];
		}

		set.Add(id);
	}

	public void OnIndexRemoved<T>(int id, T index) where T : struct, IComponent
	{
		var store = GetIndexStore<T>();

		if (store.TryGetValue(index, out var set))
		{
			set.Remove(id);

			if (set.Count == 0)
			{
				store.Remove(index);
			}
		}
	}

	public void OnEntityRecycled(int id)
	{
		foreach (var it in SetByFilter.Values)
		{
			it.Remove(id);
		}

		foreach (var store in IndexStoreByType.Values.Cast<IDictionary>())
		{
			foreach (var it in store.Values.Cast<SparseSet>())
			{
				it.Remove(id);
			}
		}
	}

	public IIndexableSet<int> View(Filter filter)
	{
		return SetByFilter[filter];
	}

	public IIndexableSet<int> View<T>(T index) where T : struct, IComponent
	{
		return GetIndexStore<T>().TryGetValue(index, out var set) ? set : SparseSet.Empty;
	}
}
