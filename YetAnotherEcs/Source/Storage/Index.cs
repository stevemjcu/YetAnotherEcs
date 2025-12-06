namespace YetAnotherEcs.Storage;

internal class Index
{
	private readonly Dictionary<Filter, HashSet<int>> SetByFilter = [];
	private readonly Dictionary<int, HashSet<int>> SetByHash = [];
	private readonly HashSet<int> Empty = [];

	public Index(Registry registry)
	{
		registry.StructureChanged += OnStructureChanged;
		registry.ValueAdded += OnValueAdded;
		registry.ValueRemoved += OnValueRemoved;
		registry.EntityRecycled += OnEntityRecycled;
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

	public void Register(Filter filter)
	{
		SetByFilter[filter] = [];
	}

	public IReadOnlySet<int> View(Filter filter)
	{
		return SetByFilter[filter];
	}

	public IReadOnlySet<int> View<T>(T index) where T : struct
	{
		var any = SetByHash.TryGetValue(Registry.Hash(index), out var set);
		return any ? set! : Empty;
	}
}
