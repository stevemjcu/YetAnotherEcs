namespace YetAnotherEcs;

/// <summary>
/// Manages all storage for the ECS.
/// </summary>
public class World
{
	private readonly int Id;

	private static readonly IdAssigner WorldIdAssigner = new();
	internal static readonly List<World> WorldById = [];

	private readonly IdAssigner EntityIdAssigner = new();
	private readonly List<Entity> EntityById = [];

	private readonly List<int> BitmaskByEntityId = [];
	private readonly List<object> ComponentStoreByTypeId = [];

	public World()
	{
		Id = WorldIdAssigner.Assign();
		WorldById.EnsureCapacity(Id + 1);
		WorldById[Id] = this;
	}

	~World() => WorldIdAssigner.Recycle(Id);

	#region World API

	/// <summary>
	/// Create an entity.
	/// </summary>
	/// <returns>The entity.</returns>
	public Entity Create()
	{
		var id = EntityIdAssigner.Assign();
		EntityById.EnsureCapacity(id + 1);
		EntityById[id] = new(id, Id, EntityById[id].Version + 1);

		return EntityById[id];
	}

	public Entity Clone(Entity entity) => throw new NotImplementedException();

	/// <summary>
	/// Destroy an entity.
	/// </summary>
	/// <param name="entity">The entity.</param>
	public void Destroy(Entity entity)
	{
		BitmaskByEntityId[Id] = 0;
		EntityIdAssigner.Recycle(entity.Id);
	}

	public void Index<T>() where T : struct => throw new NotImplementedException();

	public void Filter() => throw new NotImplementedException();

	public Entity Single<T>() where T : struct => throw new NotImplementedException();

	public Entity Single<T>(T index) where T : struct => throw new NotImplementedException();

	public IReadOnlySet<Entity> Get<T>(T index) where T : struct => throw new NotImplementedException();

	public IReadOnlySet<Entity> Get(Filter filter) => throw new NotImplementedException();

	#endregion

	#region Entity API

	internal Entity Set<T>(Entity entity, T component = default) where T : struct
	{
		var id = TypeIdAssigner1<T>.Id;
		BitmaskByEntityId[entity.Id] &= 1 << id;
		((ComponentStore<T>)ComponentStoreByTypeId[id]).Set(entity.Id, component);

		return entity;
	}

	#endregion
}
