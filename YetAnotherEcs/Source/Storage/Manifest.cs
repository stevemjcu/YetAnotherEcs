using System.Collections;
using YetAnotherEcs.General;

namespace YetAnotherEcs.Storage;

internal class Manifest(World World)
{
	private static readonly SparseSet Empty = [];

	private readonly Dictionary<Filter, SparseSet> IdSetByFilter = [];
	private readonly Dictionary<int, object> IndexStoreByType = [];

	// Rename to signatures? An index is sort of a filter too.
	private readonly HashSet<Filter> Filters = [];
	private readonly HashSet<int> Indexes = [];

	private Registry Registry => World.Registry;

	public void OnStructureChanged(Entity entity)
	{
		foreach (var it in IdSetByFilter)
		{
			if (it.Key.Compare(entity.Bitmask))
			{
				it.Value.Add(entity.Id);
			}
			else
			{
				it.Value.Remove(entity.Id);
			}
		}
	}

	public void OnIndexAdded<T>(Entity entity) where T : struct
	{
		var store = GetIndexStore<T>();
		var index = entity.Get<T>();

		if (!store.TryGetValue(index, out var set))
		{
			store[index] = set = [];
		}

		set.Add(entity.Id);
	}

	public void OnIndexRemoved<T>(Entity entity) where T : struct
	{
		var store = GetIndexStore<T>();
		var index = entity.Get<T>();

		if (store.TryGetValue(index, out var set))
		{
			set.Remove(entity.Id);
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

		// Maps component to entity set
		return (Dictionary<T, SparseSet>)value;
	}

	private void Build(Filter filter)
	{
		IdSetByFilter[filter] = [];

		foreach (var it in Registry.GetEntities())
		{
			OnStructureChanged(it);
		}
	}

	private void Build<T>() where T : struct
	{
		if (!Component<T>.Indexed)
		{
			throw new InvalidOperationException(
				$"Cannot build an index for the non-indexed component {typeof(T)}.");
		}

		foreach (var it in Registry.GetEntities())
		{
			if (it.Has<T>())
			{
				OnIndexAdded<T>(it);
			}
		}
	}
}
