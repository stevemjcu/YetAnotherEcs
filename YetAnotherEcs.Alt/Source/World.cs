using YetAnotherEcs.Storage;

namespace YetAnotherEcs;

public class World
{
	internal Registry Entities;
	internal Storage.Index Queries;

	public World()
	{
		Entities = new();
		Queries = new(Entities);
	}

	public int Create() => Entities.Create();

	public void Destroy(int id) => Entities.Destroy(id);

	public bool IsAlive(int id) => Entities.IsAlive(id);

	public void Set<T>(int id, T value = default) where T : struct => Entities.Set(id, value);

	public void Remove<T>(int id) where T : struct => Entities.Remove<T>(id);

	public bool Has<T>(int id) where T : struct => Entities.Has<T>(id);

	public T Get<T>(int id) where T : struct => Entities.Get<T>(id);
	
	public void Index(Filter filter) => Queries.Register(filter);

	public void Index<T>() where T : struct => Entities.Flag<T>();

	public IReadOnlySet<int> Query(Filter filter) => Queries.Query(filter);

	public IReadOnlySet<int> Query<T>(T index) where T : struct => Queries.Query(index);
}
