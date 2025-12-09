using YetAnotherEcs.General;
using YetAnotherEcs.Storage;

namespace YetAnotherEcs;

public class World
{
	internal Registry Registry;
	internal Manifest Manifest;

	public World()
	{
		Registry = new(this);
		Manifest = new(this);
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

	public bool TryGet<T>(int id, out T value) where T : struct
	{
		return Registry.TryGet<T>(id, out value);
	}

	public IIndexableSet<int> View(Filter filter)
	{
		return Manifest.View(filter);
	}

	public IIndexableSet<int> View<T>(T index) where T : struct
	{
		return Manifest.View(index);
	}
}
