namespace YetAnotherEcs.Storage;

internal class QueryStore
{
	private readonly Dictionary<Filter, HashSet<int>> SetByFilter = [];
	private readonly Dictionary<int, HashSet<int>> SetByIndex = [];

	public QueryStore(EntityStore store)
	{
		store.BitmaskChanged += OnBitmaskChanged;
		store.IndexChanged += OnIndexChanged;
		store.EntityDestroyed += OnEntityDestroyed;
	}

	private void OnBitmaskChanged(int id, int bitmask)
	{
		foreach (var it in SetByFilter)
		{
			if (it.Key.Compare(bitmask)) it.Value.Add(id);
			else it.Value.Remove(id);
		}
	}

	private void OnIndexChanged(int id, int index1, int index2)
	{
		SetByIndex[index1].Remove(id);
		SetByIndex[index2].Add(id);
	}

	private void OnEntityDestroyed(int id)
	{
		foreach (var it in SetByFilter.Values) it.Remove(id);
		foreach (var it in SetByIndex.Values) it.Remove(id);
	}

	public IReadOnlySet<int> Query(Filter filter) => SetByFilter[Filter];

	public IReadOnlySet<int> Query<T>(T index) => SetByIndex[EntityStore.Hash(index)];
}
