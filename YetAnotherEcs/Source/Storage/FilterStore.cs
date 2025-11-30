namespace YetAnotherEcs.Storage;

/// <summary>
/// Encapsulates the storage for all filter signatures.
/// </summary>
internal class FilterStore
{
	private readonly Dictionary<Filter, HashSet<Entity>> EntitySetByFilter = [];

	public bool Contains(Filter filter) => EntitySetByFilter.ContainsKey(filter);

	public void Add(Filter filter, HashSet<Entity> initial) => EntitySetByFilter[filter] = initial;
}
