namespace YetAnotherEcs.Storage;

internal class Index
{
	private readonly Dictionary<Filter, HashSet<int>> SetByFilter = [];
	private readonly Dictionary<int, HashSet<int>> SetByHash = [];

	public Index(Registry registry)
	{
		registry.StructureChanged += OnStructureChanged;
		registry.ValueChanged += OnValueChanged;
		registry.EntityDestroyed += OnEntityDestroyed;
	}

	private void OnStructureChanged(int id, int bitmask)
	{
		foreach (var it in SetByFilter)
		{
			if (it.Key.Compare(bitmask)) it.Value.Add(id);
			else it.Value.Remove(id);
		}
	}

	private void OnValueChanged(int id, int index1, int index2)
	{
		if (SetByHash.TryGetValue(index1, out var set1))
		{
			set1.Remove(id);
			if (set1.Count == 0) SetByHash.Remove(index1);
		}

		if (SetByHash.TryGetValue(index1, out var set2)) set2.Add(id);
		else SetByHash[index2] = [id];
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
