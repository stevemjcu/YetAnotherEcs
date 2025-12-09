using System.Collections;
using YetAnotherEcs.General;

namespace YetAnotherEcs.Storage;

internal class Manifest
{
	private static readonly SparseSet Empty = [];

	private readonly Dictionary<Filter, SparseSet> IdSetByFilter = [];
	private readonly Dictionary<int, object> IndexStoreByType = [];

	private int IndexBitmask;

	public void Index<T>() where T : struct
	{
		IndexBitmask |= Component<T>.Bitmask;
	}

	public bool IsIndexed<T>() where T : struct
	{
		return (IndexBitmask & Component<T>.Bitmask) > 0;
	}

	public void OnStructureChanged(int id, int bitmask)
	{
		foreach (var it in IdSetByFilter)
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

	public void OnIndexAdded<T>(int id, T index) where T : struct
	{
		var store = GetIndexStore<T>();

		if (!store.TryGetValue(index, out var set))
		{
			store[index] = set = [];
		}

		set.Add(id);
	}

	public void OnIndexRemoved<T>(int id, T index) where T : struct
	{
		var store = GetIndexStore<T>();

		if (store.TryGetValue(index, out var set))
		{
			set.Remove(id);
		}
	}

	public void OnEntityRecycled(int id)
	{
		foreach (var it in IdSetByFilter.Values)
		{
			it.Remove(id);
		}

		foreach (IDictionary store in IndexStoreByType.Values)
		{
			foreach (SparseSet it in store.Values)
			{
				it.Remove(id);
			}
		}
	}

	public IIndexableSet<int> View(Filter filter)
	{
		// TODO: Build up if first time calling
		return IdSetByFilter.TryGetValue(filter, out var set) ? set : Empty;
	}

	public IIndexableSet<int> View<T>(T index) where T : struct
	{
		// TODO: Build up if first time calling
		return GetIndexStore<T>().TryGetValue(index, out var set) ? set : Empty;
	}

	private Dictionary<T, SparseSet> GetIndexStore<T>() where T : struct
	{
		var typeId = Component<T>.Id;

		if (!IndexStoreByType.TryGetValue(typeId, out var value))
		{
			value = new Dictionary<T, SparseSet>();
			IndexStoreByType.Add(typeId, value);
		}

		// entity set by component
		return (Dictionary<T, SparseSet>)value;
	}
}
