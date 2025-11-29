using YetAnotherEcs.Storage;

namespace YetAnotherEcs;

/// <summary>
/// Encapsulates the storage for all objects in the system.
/// </summary>
public class World
{
	internal EntityStore Entities = new();
	internal ComponentStore Components = new();
	internal FilterStore Filters = new();

	/// <summary>
	/// Create an entity.
	/// </summary>
	/// <returns>The new entity.</returns>
	public Entity Create() => Entities.Add(this);

	/// <summary>
	/// Create a filter.
	/// </summary>
	/// <returns>The filter.</returns>
	public Filter Filter() => Filters.Add(this);

	/// <summary>
	/// Get an entity by a distinctive component type.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	/// <returns>The entity.</returns>
	public Entity Singleton<T>() where T : struct, IComponent<T> =>
		Entities.Get(Components.Get<T>());
}
