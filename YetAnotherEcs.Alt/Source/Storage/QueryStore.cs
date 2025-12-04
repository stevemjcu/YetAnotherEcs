namespace YetAnotherEcs.Storage;

internal class QueryStore
{
	// include bitmask, exclude bitmask
	private Dictionary<(int, int), HashSet<int>> Filters = [];

	// type id, value hash
	private Dictionary<(int, int), HashSet<int>> Indexes = [];

	public QueryStore(EntityStore pool)
	{
		pool.BitmaskChanged += OnBitmaskChanged;
		pool.IndexChanged += OnIndexChanged;
	}

	private void OnBitmaskChanged(int id, int bitmask) => throw new NotImplementedException();

	private void OnIndexChanged(int id, int typeId, int a, int b) => throw new NotImplementedException();
}
