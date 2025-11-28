using YetAnotherEcs.General;
using YetAnotherEcs.Storage;

namespace YetAnotherEcs;

/// <summary>
/// Manages all storage for the ECS.
/// </summary>
public class World
{
	private static readonly IdAssigner WorldIdAssigner = new();
	private static readonly List<World> WorldById = [];

	private int Id;
	private List<int> BitmaskByEntityId = [];
	private List<ComponentStore> ComponentStoreByTypeId = [];

	public World()
	{
		Id = WorldIdAssigner.Assign();
		WorldById.EnsureCapacity(Id + 1);
		WorldById[Id] = this;
	}

	~World() => WorldIdAssigner.Recycle(Id);

	public Entity Create() => default;

	public Entity Clone(Entity entity) => default;

	public void Destroy(Entity entity) { }

	public void Index<T>() { }

	public void Filter() { }

	public Entity Get<T>() => default;

	public Entity Get<T>(T index) => default;

	public Entity Get(Filter filter) => default;
}
