namespace YetAnotherEcs.Storage;

/// <summary>
/// Manages the storage for all components.
/// </summary>
public class ComponentStore
{
	private int NextTypeId = 0;
	private readonly Dictionary<Type, int> TypeIdByType = [];
	private readonly List<object> StoreByTypeId = [];

	public void Set<T>(Entity entity, T component) where T : struct, IComponent<T>
	{
		var (id, store) = Expand<T>();

		entity.Bitmask |= 1 << id;
		store[entity.Id] = component;
	}

	public void Remove<T>(Entity entity) where T : struct, IComponent<T>
	{
		var (id, store) = Expand<T>();

		store.Remove(entity.Id);
		entity.Bitmask ^= 1 << id;
	}

	public bool Has<T>(Entity entity) where T : struct, IComponent<T> =>
		(entity.Bitmask & (1 << Expand<T>().Id)) > 0;

	public T Get<T>(Entity entity) where T : struct, IComponent<T> =>
		Expand<T>().Store[entity.Id];

	// TODO: Profile to determine if a static ID would be much better.
	private (int Id, Dictionary<int, T> Store) Expand<T>() where T : struct, IComponent<T>
	{
		Register<T>(out var type);

		var id = TypeIdByType[type];
		var store = (Dictionary<int, T>)StoreByTypeId[id];

		return (id, store);
	}

	private void Register<T>(out Type type) where T : struct, IComponent<T>
	{
		type = typeof(T);
		if (TypeIdByType.ContainsKey(type)) return;
		var id = NextTypeId++;

		TypeIdByType[type] = id;
		StoreByTypeId.Add(new Dictionary<int, T>());
	}
}