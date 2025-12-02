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
	/// Get an entity by its ID.
	/// </summary>
	/// <param name="id">The entity's ID.</param>
	/// <returns>The entity.</returns>
	public Entity Get(int id) => Entities.Get(id);

	/// <summary>
	/// Create an entity.
	/// </summary>
	/// <returns>The new entity.</returns>
	public Entity Create() => Entities.Add(this);

	/// <summary>
	/// Create an empty filter.
	/// </summary>
	/// <returns>The filter.</returns>
	public Filter Filter() => new(this);

	/// <summary>
	/// Create an index.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	public Index<T> Index<T>() where T : struct
	{
		Components.Index<T>();
		return new();
	}
}
