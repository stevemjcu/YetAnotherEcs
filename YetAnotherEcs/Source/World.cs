namespace YetAnotherEcs;

/// <summary>
/// Manages all storage for the ECS.
/// </summary>
public class World
{
	private readonly IdAssigner EntityIdAssigner = new();
	private readonly List<Entity> EntityById = [];

	private readonly List<int> BitmaskByEntityId = [];
	private readonly List<object> ComponentStoreByTypeId = [];

	#region World API

	/// <summary>
	/// Create an entity.
	/// </summary>
	/// <returns>The entity.</returns>
	public Entity Create()
	{
		var id = EntityIdAssigner.Assign();

		EntityById.EnsureCapacity(id + 1);
		EntityById[id] = new(id, EntityById[id].Version + 1, this);

		return EntityById[id];
	}

	public Entity Clone(Entity entity) =>
		throw new NotImplementedException();

	/// <summary>
	/// Destroy an entity.
	/// </summary>
	/// <param name="entity">The entity.</param>
	public void Destroy(Entity entity)
	{
		BitmaskByEntityId[entity.Id] = 0;
		EntityIdAssigner.Recycle(entity.Id);
	}

	public void Index<T>() where T : struct, IComponent<T> =>
		throw new NotImplementedException();

	public void Filter() =>
		throw new NotImplementedException();

	public Entity Single<T>() where T : struct, IComponent<T> =>
		throw new NotImplementedException();

	public Entity Single<T>(T index) where T : struct, IComponent<T> =>
		throw new NotImplementedException();

	public IReadOnlySet<Entity> Get<T>(T index) where T : struct, IComponent<T> =>
		throw new NotImplementedException();

	public IReadOnlySet<Entity> Get(Filter filter) =>
		throw new NotImplementedException();

	#endregion

	#region Entity API

	internal Entity Set<T>(Entity entity, T component = default) where T : struct, IComponent<T>
	{
		var id = IComponent<T>.Id;
		var store = (ComponentStore<T>)ComponentStoreByTypeId[id];

		BitmaskByEntityId[entity.Id] &= 1 << id;
		store.Set(entity.Id, component);

		return entity;
	}

	#endregion
}
