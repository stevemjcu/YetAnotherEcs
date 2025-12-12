using System.Runtime.InteropServices;
using YetAnotherEcs.General;
using YetAnotherEcs.Storage;

namespace YetAnotherEcs;

public class World : IDisposable
{
	internal Registry Registry;
	internal Manifest Manifest;

	private static readonly IdPool IdPool = new();
	internal static readonly List<World?> WorldById = [];
	internal readonly int Id;

	public World()
	{
		Id = IdPool.Assign();

		if (WorldById.Count < Id + 1)
		{
			CollectionsMarshal.SetCount(WorldById, Id + 1);
		}

		WorldById[Id] = this;

		Registry = new(this);
		Manifest = new(this);
	}

	public void Dispose()
	{
		IdPool.Recycle(Id);
		WorldById[Id] = null;
		GC.SuppressFinalize(this);
	}

	public Entity Create()
	{
		return Registry.Create();
	}

	public void Destroy(Entity entity)
	{
		Registry.Destroy(entity.Id);
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
