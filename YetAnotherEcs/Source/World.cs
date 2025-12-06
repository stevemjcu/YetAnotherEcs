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

	public void Recycle(int id)
	{
		Registry.Recycle(id);
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

	public (T, U) Get<T, U>(int id)
		where T : struct
		where U : struct
	{
		return (Registry.Get<T>(id), Registry.Get<U>(id));
	}

	public (T, U, V) Get<T, U, V>(int id)
		where T : struct
		where U : struct
		where V : struct
	{
		var (a, b) = Get<T, U>(id);
		return (a, b, Registry.Get<V>(id));
	}

	public Span<int> View(Filter filter)
	{
		return Index.View(filter);
	}

	public Span<int> View<T>(T index) where T : struct
	{
		return Index.View(index);
	}
}
