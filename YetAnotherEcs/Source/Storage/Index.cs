using YetAnotherEcs.General;

namespace YetAnotherEcs.Storage;

internal class Index
{
	private readonly Registry Registry;
	private readonly Dictionary<Filter, SparseSet> SetByFilter = [];
	private readonly Dictionary<int, SparseSet> SetByHash = [];
	private readonly SparseSet Empty = [];

	public Index(Registry registry)
	{
		Registry = registry;
		Registry.StructureChanged += OnStructureChanged;
		Registry.ValueAdded += OnValueAdded;
		Registry.ValueRemoved += OnValueRemoved;
		Registry.EntityRecycled += OnEntityRecycled;
	}

	private void OnStructureChanged(int id, int bitmask)
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

	private void OnValueAdded(int id, int hash)
	{
		if (!SetByHash.TryGetValue(hash, out var set))
		{
			SetByHash[hash] = set = [];
		}

		set.Add(id);
	}

	private void OnValueRemoved(int id, int hash)
	{
		if (SetByHash.TryGetValue(hash, out var set))
		{
			set.Remove(id);

			if (set.Count == 0)
			{
				SetByHash.Remove(hash);
			}
		}
	}

	private void OnEntityRecycled(int id)
	{
		foreach (var it in SetByFilter.Values)
		{
			it.Remove(id);
		}

		foreach (var it in SetByHash.Values)
		{
			it.Remove(id);
		}
	}

	public IIndexableSet<int> View(Filter filter)
	{
		if (!SetByFilter.TryGetValue(filter, out var set))
		{
			Build(filter);
			set = SetByFilter[filter];
		}

		return set;
	}

	public IIndexableSet<int> View<T>(T index) where T : struct
	{
		if (!Registry.IsFlagged<T>())
		{
			Build<T>();
		}

		var hash = Registry.Hash(index);
		return SetByHash.TryGetValue(hash, out var set) ? set : Empty;
	}

	private void Build(Filter filter)
	{
		SetByFilter[filter] = [];

		foreach (var (id, bitmask) in Registry.Enumerate())
		{
			OnStructureChanged(id, bitmask);
		}
	}

	private void Build<T>() where T : struct
	{
		Registry.Flag<T>();

		foreach (var (id, _) in Registry.Enumerate())
		{
			if (Registry.Has<T>(id))
			{
				var hash = Registry.Hash(Registry.Get<T>(id));
				OnValueAdded(id, hash);
			}
		}
	}
}
