namespace YetAnotherEcs.Alt.Storage;

internal class QueryPool
{
	// include bitmask, exclude bitmask
	private Dictionary<(int, int), HashSet<int>> Filters = [];

	// type id, value hash
	private Dictionary<(int, int), HashSet<int>> Indexes = [];

	public QueryPool(EntityPool pool)
	{
		pool.BitmaskChanged += OnBitmaskChanged;
		pool.IndexChanged += OnIndexChanged;
	}

	private void OnBitmaskChanged(int id, int bitmask) => throw new NotImplementedException();

	private void OnIndexChanged(int id, int typeId, int a, int b) => throw new NotImplementedException();
}
