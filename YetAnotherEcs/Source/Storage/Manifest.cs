using System.Collections;
using YetAnotherEcs.General;

namespace YetAnotherEcs.Storage;

internal class Manifest(World World)
{
	private static readonly SparseSet Empty = [];

	private readonly Registry Registry = World.Registry;
	private readonly Dictionary<Filter, SparseSet> IdSetByFilter = [];
	private readonly Dictionary<int, object> IndexStoreByType = [];

	private readonly HashSet<Filter> Filters = [];
	private readonly HashSet<int> Indexes = [];

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

		// FIXME: Is this cast ok?
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
		if (!Filters.Contains(filter))
		{
			Build(filter);
			Filters.Add(filter);
		}

		return IdSetByFilter[filter];
	}

	public IIndexableSet<int> View<T>(T index) where T : struct
	{
		if (!Indexes.Contains(Component<T>.Id))
		{
			Build<T>();
			Indexes.Add(Component<T>.Id);
		}

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

	private void Build(Filter filter)
	{
		IdSetByFilter[filter] = [];

		foreach (var (id, bitmask) in Registry.GetEntities())
		{
			OnStructureChanged(id, bitmask);
		}
	}

	private void Build<T>() where T : struct
	{
		foreach (var (id, _) in Registry.GetEntities())
		{
			if (Registry.TryGet<T>(id, out var value))
			{
				OnIndexAdded<T>(id, value);
			}
		}
	}
}
