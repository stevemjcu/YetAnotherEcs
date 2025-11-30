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
	/// Copy an entity and its components.
	/// </summary>
	/// <returns>The new entity.</returns>
	public Entity Copy(Entity entity) => Entities.Copy(entity);

	/// <summary>
	/// Destroy an entity.
	/// </summary>
	public void Destroy(Entity entity) => Entities.Remove(entity);

	/// <summary>
	/// Create a filter.
	/// </summary>
	/// <returns>The filter.</returns>
	public Filter Filter() => new(this);

	/// <summary>
	/// Create an index.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	public void Index<T>() => throw new NotImplementedException();

	/// <summary>
	/// Get an entity by its id.
	/// </summary>
	/// <param name="id">The entity's id.</param>
	/// <returns>The entity.</returns>
	public Entity Get(int id) => Entities.Get(id);

	/// <summary>
	/// Get an entity by a distinctive component type.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	/// <returns>The entity.</returns>
	public Entity Get<T>() where T : struct => Entities.Get(Components.Get<T>());

	/// <summary>
	/// Get an entity by a distinctive component index.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	/// <param name="index">The indexed component.</param>
	/// <returns>The entity.</returns>
	public Entity Get<T>(T index) => throw new NotImplementedException();

	/// <summary>
	/// Get the entity set associated with a filter.
	/// </summary>
	/// <returns>The entity set.</returns>
	public IReadOnlySet<Entity> Query(Filter filter) => Filters.Query(filter);

	/// <summary>
	/// Get the entity set associated with an index.
	/// </summary>
	/// <returns>The entity set.</returns>
	public IReadOnlySet<Entity> Query<T>(T index) => throw new NotImplementedException();
}
