using System.Runtime.InteropServices;
using YetAnotherEcs.General;

namespace YetAnotherEcs.Storage;

/// <summary>
/// Manages the storage for all entities.
/// </summary>
public class EntityStore(World world)
{
	private readonly IdAssigner EntityIdAssigner = new();
	private readonly List<Entity> EntityById = [];
	private readonly List<int> BitmaskByEntityId = [];

	/// <summary>
	/// Create an entity.
	/// </summary>
	/// <returns>The entity.</returns>
	public Entity Add()
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

	internal Entity Copy(Entity entity) =>
		throw new NotImplementedException();

	internal void Destroy(Entity entity)
	{
		BitmaskByEntityId[entity.Id] = 0;
		EntityIdAssigner.Recycle(entity.Id);
	}

	internal int GetBitmask(Entity entity) => BitmaskByEntityId[entity.Id];

	internal void SetBitmask(Entity entity, int value) => BitmaskByEntityId[entity.Id] = value;
}
