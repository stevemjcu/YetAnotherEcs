using YetAnotherEcs.General;
using YetAnotherEcs.Storage;

namespace YetAnotherEcs;

/// <summary>
/// Manages all storage for the ECS.
/// </summary>
public class World
{
	private readonly int Id;

	private static readonly IdAssigner WorldIdAssigner = new();
	private static readonly List<World> WorldById = [];

	private readonly IdAssigner EntityIdAssigner = new();
	private readonly List<Entity> EntityById = [];

	private readonly List<int> BitmaskByEntityId = [];
	private readonly List<ComponentStore> ComponentStoreByTypeId = [];

	public World()
	{
		Id = WorldIdAssigner.Assign();
		WorldById.EnsureCapacity(Id + 1);
		WorldById[Id] = this;
	}

	~World() => WorldIdAssigner.Recycle(Id);

	public Entity Create()
	{
		var id = EntityIdAssigner.Assign(out var recycled);
		EntityById.EnsureCapacity(id + 1);

		var version = recycled ? EntityById[id].Version + 1 : 0;
		EntityById[id] = new(id, Id, version);

		return EntityById[id];
	}

	public Entity Clone(Entity entity) => throw new NotImplementedException();

	public void Destroy(Entity entity)
	{
		BitmaskByEntityId[Id] = 0;
		EntityIdAssigner.Recycle(entity.Id);
	}

	public void Index<T>() => throw new NotImplementedException();

	public void Filter() => throw new NotImplementedException();

	public Entity Get<T>() => throw new NotImplementedException();

	public Entity Get<T>(T index) => throw new NotImplementedException();

	public Entity Get(Filter filter) => throw new NotImplementedException();
}
