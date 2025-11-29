using System.Runtime.InteropServices;
using YetAnotherEcs.General;
using YetAnotherEcs.Storage;

namespace YetAnotherEcs;

/// <summary>
/// Manages the storage for all entities and their associated stores.
/// </summary>
public class World
{
	internal readonly List<int> BitmaskByEntityId = [];
	internal ComponentStore Components = new();

	private readonly IdAssigner EntityIdAssigner = new();
	private readonly List<Entity> EntityById = [];

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

		EntityById[id] = new(id, version, this);
		return EntityById[id];
	}

	internal Entity Copy(Entity entity) =>
		throw new NotImplementedException();

	internal void Destroy(Entity entity)
	{
		BitmaskByEntityId[entity.Id] = 0;
		EntityIdAssigner.Recycle(entity.Id);
	}

	//public Filter Filter() =>
	//	throw new NotImplementedException();

	//public Entity Single<T>() where T : struct, IComponent<T> =>
	//	throw new NotImplementedException();

	//public Entity Single<T>(T index) where T : struct, IComponent<T> =>
	//	throw new NotImplementedException();

	//public IReadOnlySet<Entity> Get<T>(T index) where T : struct, IComponent<T> =>
	//	throw new NotImplementedException();

	//public IReadOnlySet<Entity> Get(Filter filter) =>
	//	throw new NotImplementedException();
}
