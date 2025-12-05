namespace YetAnotherEcs.Storage;

internal class Index
{
	private readonly Dictionary<Filter, HashSet<int>> SetByFilter = [];
	private readonly Dictionary<int, HashSet<int>> SetByHash = [];

	public Index(Registry store)
	{
		store.StructureChanged += OnStructureChanged;
		store.ValueChanged += OnValueChanged;
		store.EntityDestroyed += OnEntityDestroyed;
	}

	private void OnStructureChanged(int id, int bitmask)
	{
		foreach (var it in SetByFilter)
		{
			if (it.Key.Compare(bitmask)) it.Value.Add(id);
			else it.Value.Remove(id);
		}
	}

	// TODO: Ensure set exists before operating on it, and remove if empty
	private void OnValueChanged(int id, int index1, int index2)
	{
		SetByHash[index1].Remove(id);
		SetByHash[index2].Add(id);
	}

	private void OnEntityDestroyed(int id)
	{
		foreach (var it in SetByFilter.Values) it.Remove(id);
		foreach (var it in SetByHash.Values) it.Remove(id);
	}

	public void Register(Filter filter) => SetByFilter[filter] = [];

	public IReadOnlySet<int> Query(Filter filter) => SetByFilter[filter];

	public IReadOnlySet<int> Query<T>(T index) where T : struct => SetByHash[Registry.Hash(index)];
}
