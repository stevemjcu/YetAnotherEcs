using YetAnotherEcs.General;

namespace YetAnotherEcs.Storage;

internal class Index
{
	private readonly Registry Registry;
	private readonly Dictionary<Filter, SparseSet> SetByFilter = [];
	private readonly Dictionary<int, SparseSet> SetByHash = [];
	private readonly SparseSet Empty = new();

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
			SetByHash[hash] = set = new();
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

	public Span<int> View(Filter filter)
	{
		if (!SetByFilter.TryGetValue(filter, out var set))
		{
			set = new();
			SetByFilter[filter] = set;
			// TODO: For each entity, try adding to set
		}

		return set.AsSpan();
	}

	public Span<int> View<T>(T index) where T : struct
	{
		if (!Registry.IsFlagged<T>())
		{
			Registry.Flag<T>();
			// TODO: For each entity, try adding to set
		}

		return SetByHash.TryGetValue(Registry.Hash(index), out var set)
			? set.AsSpan() : Empty.AsSpan();
	}
}
