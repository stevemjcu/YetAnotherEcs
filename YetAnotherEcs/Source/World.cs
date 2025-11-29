using YetAnotherEcs.Storage;

namespace YetAnotherEcs;

/// <summary>
/// Manages a set of entities and their storage. 
/// </summary>
public class World
{
	internal EntityStore Entities = new();
	internal ComponentStore Components = new();
	internal FilterStore Filters = new();

	/// <summary>
	/// Create an entity.
	/// </summary>
	/// <returns>The entity.</returns>
	public Entity Create() => Entities.Add(this);

	/// <summary>
	/// Create a filter.
	/// </summary>
	/// <returns>The filter.</returns>
	public Filter Filter() => Filters.Add(this);

	/// <summary>
	/// Get an entity by a distinctive component type.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public Entity Singleton<T>() where T : struct, IComponent<T> =>
		throw new NotImplementedException();
}
