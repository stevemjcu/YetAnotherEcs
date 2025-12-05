using YetAnotherEcs.Storage;
using Index = YetAnotherEcs.Storage.Index;

namespace YetAnotherEcs;

public class World
{
	internal Registry Registry;
	internal Index Index;

	public World()
	{
		Registry = new();
		Index = new(Registry);
	}

	public int Create()
	{
		return Registry.Create();
	}

	public void Destroy(int id)
	{
		Registry.Destroy(id);
	}

	public bool IsAlive(int id)
	{
		return Registry.IsAlive(id);
	}

	public void Set<T>(int id, T value = default) where T : struct
	{
		Registry.Set(id, value);
	}

	public void Remove<T>(int id) where T : struct
	{
		Registry.Remove<T>(id);
	}

	public bool Has<T>(int id) where T : struct
	{
		return Registry.Has<T>(id);
	}

	public T Get<T>(int id) where T : struct
	{
		return Registry.Get<T>(id);
	}

	public void IndexOn(Filter filter)
	{
		Index.Register(filter);
	}

	public void IndexOn<T>() where T : struct
	{
		Registry.Flag<T>();
	}

	public IReadOnlySet<int> Query(Filter filter)
	{
		return Index.Query(filter);
	}

	public IReadOnlySet<int> Query<T>(T index) where T : struct
	{
		return Index.Query(index);
	}
}
