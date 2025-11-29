using System.Runtime.InteropServices;

namespace YetAnotherEcs;

/// <summary>
/// Manages all storage for the ECS.
/// </summary>
public class World
{
	private readonly IdAssigner EntityIdAssigner = new();
	private readonly List<Entity> EntityById = [];
	private readonly List<int> BitmaskByEntityId = [];

	private int NextComponentTypeId = 0;
	private readonly Dictionary<Type, int> ComponentTypeIdByType = [];
	private readonly List<object> ComponentStoreByTypeId = [];

	#region World API

	/// <summary>
	/// Register a component type.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	/// <param name="index">Maintain an index for entity lookup.</param>
	public void Register<T>(bool index = false) where T : struct, IComponent<T>
	{
		var type = typeof(T);
		if (ComponentTypeIdByType.ContainsKey(type)) return;
		var id = NextComponentTypeId++;

		ComponentTypeIdByType[type] = id;
		ComponentStoreByTypeId.Add(new ComponentStore<T>());
	}

	/// <summary>
	/// Create an entity.
	/// </summary>
	/// <returns>The entity.</returns>
	public Entity Create()
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

	public Filter Filter() =>
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

	internal Entity Copy(Entity entity) =>
		throw new NotImplementedException();

	internal void Destroy(Entity entity)
	{
		BitmaskByEntityId[entity.Id] = 0;
		EntityIdAssigner.Recycle(entity.Id);
	}

	internal Entity Set<T>(Entity entity, T component = default) where T : struct, IComponent<T>
	{
		var (id, store) = Expand<T>();

		BitmaskByEntityId[entity.Id] |= 1 << id;
		store.Set(entity.Id, component);

		return entity;
	}

	internal void Remove<T>(Entity entity) where T : struct, IComponent<T>
	{
		var (id, store) = Expand<T>();
		store.Remove(entity.Id);

		BitmaskByEntityId[entity.Id] ^= 1 << id;
	}

	internal bool Has<T>(Entity entity) where T : struct, IComponent<T>
	{
		var (id, _) = Expand<T>();
		var bitmask = BitmaskByEntityId[entity.Id];

		return (bitmask & (1 << id)) > 0;
	}

	internal T Get<T>(Entity entity) where T : struct, IComponent<T>
	{
		var (_, store) = Expand<T>();

		return store.Get(entity.Id);
	}

	#endregion

	// TODO: Profile to determine if a static ID would be much better.
	private (int, ComponentStore<T>) Expand<T>() where T : struct, IComponent<T>
	{
		var id = ComponentTypeIdByType[typeof(T)];
		var store = (ComponentStore<T>)ComponentStoreByTypeId[id];

		return (id, store);
	}
}
