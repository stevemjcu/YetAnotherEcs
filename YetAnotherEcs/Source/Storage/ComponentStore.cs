namespace YetAnotherEcs;

/// <summary>
/// Manages the storage for a component type.
/// </summary>
internal class ComponentStore<T> where T : struct, IComponent<T>
{
	private readonly Dictionary<int, T> ComponentByEntityId = [];

	public void Set(int id, T component) => ComponentByEntityId[id] = component;

	public T Get(int id) => ComponentByEntityId[id];
}