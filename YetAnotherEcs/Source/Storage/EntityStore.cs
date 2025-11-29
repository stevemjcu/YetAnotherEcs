using System.Runtime.InteropServices;
using YetAnotherEcs.General;

namespace YetAnotherEcs.Storage;

/// <summary>
/// Encapsulates the storage for all entities.
/// </summary>
internal class EntityStore
{
	private readonly IdAssigner EntityIdAssigner = new();
	private readonly List<Entity> EntityById = [];
	private readonly List<int> BitmaskByEntityId = [];

	public Entity Add(World world)
	{
		var id = EntityIdAssigner.Assign(out var recycled);
		var version = recycled ? EntityById[id].Version + 1 : 0;

		if (EntityById.Count < id + 1)
		{
			CollectionsMarshal.SetCount(EntityById, id + 1);
			CollectionsMarshal.SetCount(BitmaskByEntityId, id + 1);
		}

		EntityById[id] = new(id, version, world);
		return EntityById[id];
	}

	public Entity Copy(Entity entity) =>
		throw new NotImplementedException();

	public void Destroy(Entity entity)
	{
		BitmaskByEntityId[entity.Id] = 0;
		EntityIdAssigner.Recycle(entity.Id);
	}

	internal int GetBitmask(Entity entity) =>
		BitmaskByEntityId[entity.Id];

	internal void SetBitmask(Entity entity, int value) =>
		BitmaskByEntityId[entity.Id] = value;
}
