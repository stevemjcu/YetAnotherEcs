namespace YetAnotherEcs.Storage;

/// <summary>
/// Encapsulates the storage for all filter signatures.
/// </summary>
internal class FilterStore
{
	private readonly Dictionary<Filter, HashSet<int>> IdSetByFilter = [];

	public void Add(Filter filter) => IdSetByFilter[filter] = [];

	public bool Contains(Filter filter) => IdSetByFilter.ContainsKey(filter);

	public HashSet<int>.Enumerator GetEnumerator(Filter filter) => IdSetByFilter[filter].GetEnumerator();

	public void Evaluate(int id, int bitmask)
	{
		foreach (var (k, v) in IdSetByFilter)
		{
			if (!k.Matches(bitmask)) v.Remove(id);
			else if (k.Matches(bitmask)) v.Add(id);
		}
	}
}
