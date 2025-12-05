using YetAnotherEcs.Storage;

namespace YetAnotherEcs;

public class World
{
	internal EntityStore Entities;
	internal QueryStore Queries;

	public World()
	{
		Entities = new();
		Queries = new(Entities);
	}

	public int Create() => Entities.Create();

	public void Destroy(int id) => Entities.Destroy(id);

	public bool Exists(int id) => Entities.Exists(id);

	public void Set<T>(int id, T value) where T : struct => Entities.Set(id, value);

	public void Remove<T>(int id) where T : struct => Entities.Remove<T>(id);

	public bool Has<T>(int id) where T : struct => Entities.Has<T>(id);

	public T Get<T>(int id) where T : struct => Entities.Get<T>(id);

	public FilterBuilder Filter() => new(this);

	public void Index<T>() where T : struct => Entities.Index<T>();

	public IReadOnlySet<int> Query(Filter filter) => throw new NotImplementedException();

	public IReadOnlySet<int> Query<T>(T index) => throw new NotImplementedException();
}
