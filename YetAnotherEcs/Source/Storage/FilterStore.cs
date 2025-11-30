namespace YetAnotherEcs.Storage;

/// <summary>
/// Encapsulates the storage for all filter signatures.
/// </summary>
internal class FilterStore
{
	private readonly Dictionary<Filter, HashSet<Entity>> EntitySetByFilter = [];

	public void Add(Filter filter) =>
		EntitySetByFilter[filter] = [];

	public bool Contains(Filter filter) =>
		EntitySetByFilter.ContainsKey(filter);

	public IReadOnlySet<Entity> Query(Filter filter) =>
		EntitySetByFilter[filter];

	public bool AddEntity(Filter filter, Entity entity) =>
		EntitySetByFilter[filter].Add(entity);

	public bool RemoveEntity(Filter filter, Entity entity) =>
		EntitySetByFilter[filter].Remove(entity);

	public Dictionary<Filter, HashSet<Entity>>.KeyCollection.Enumerator GetEnumerator() =>
		EntitySetByFilter.Keys.GetEnumerator();
}
