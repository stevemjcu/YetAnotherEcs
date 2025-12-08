using YetAnotherEcs.General;
using YetAnotherEcs.Storage;
using Index = YetAnotherEcs.Storage.Index;

namespace YetAnotherEcs;

public class World
{
	internal Index Index;
	internal Registry Registry;

	public World()
	{
		Index = new();
		Registry = new(Index);
	}

	public int Create()
	{
		return Registry.Create();
	}

	public void Recycle(int id)
	{
		Registry.Recycle(id);
	}

	public void Set<T>(int id, T value = default) where T : struct, IComponent
	{
		Registry.Set(id, value);
	}

	public void Remove<T>(int id) where T : struct, IComponent
	{
		Registry.Remove<T>(id);
	}

	public bool Has<T>(int id) where T : struct, IComponent
	{
		return Registry.Has<T>(id);
	}

	public T Get<T>(int id) where T : struct, IComponent
	{
		return Registry.Get<T>(id);
	}

	public bool TryGet<T>(int id, out T value) where T : struct, IComponent
	{
		var has = Registry.Has<T>(id);
		value = has ? Registry.Get<T>(id) : default;
		return has;
	}

	public IIndexableSet<int> View(Filter filter)
	{
		return Index.View(filter);
	}

	public IIndexableSet<int> View<T>(T index) where T : struct, IComponent
	{
		return Index.View(index);
	}
}
