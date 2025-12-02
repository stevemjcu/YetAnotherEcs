using System.Runtime.InteropServices;
using YetAnotherEcs.General;

namespace YetAnotherEcs.Storage;

/// <summary>
/// Encapsulates the storage for all entities.
/// </summary>
internal class EntityStore
{
	private readonly IdPool EntityIdPool = new();
	private readonly List<Entity> EntityById = [];
	private readonly List<int> BitmaskById = [];

	public Entity Add(World world)
	{
		var id = EntityIdPool.Assign(out var recycled);
		var version = recycled ? EntityById[id].Version + 1 : 0;

		if (EntityById.Count < id + 1)
		{
			CollectionsMarshal.SetCount(EntityById, id + 1);
			CollectionsMarshal.SetCount(BitmaskById, id + 1);
		}

		EntityById[id] = new(id, version, world);
		return EntityById[id];
	}

	public Entity Get(int id) => EntityById[id];

	public void Recycle(int id) => EntityIdPool.Recycle(id);

	public List<Entity>.Enumerator GetEnumerator() => EntityById.GetEnumerator();

	public int GetBitmask(int id) => BitmaskById[id];

	public void SetBitmask(int id, int value) => BitmaskById[id] = value;
}
