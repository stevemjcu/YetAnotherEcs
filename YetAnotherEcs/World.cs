using YetAnotherEcs.Storage;
using Index = YetAnotherEcs.Storage.Index;

namespace YetAnotherEcs;

/// <summary>
/// Represents the storage for the entity component system.
/// </summary>
public class World
{
	internal readonly Table Table = new();
	internal readonly Index Index = new();

	/// <summary>
	/// Creates an entity with a unique ID.
	/// </summary>
	/// <returns>The entity.</returns>
	public Entity Create()
	{
		return new(this, Table.CreateEntity());
	}

	/// <summary>
	/// Deletes an entity and recycles its ID.
	/// </summary>
	/// <param name="entity">The entity.</param>
	public void Delete(Entity entity)
	{
		Table.DeleteEntity(entity.Id);
		Index.OnEntityDeleted(entity.Id);
	}

	/// <summary>
	/// Creates an entity with the same components.
	/// </summary>
	/// <param name="entity">The original entity.</param>
	/// <returns>The new entity.</returns>
	public Entity Clone(Entity entity)
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// Enables an index for a component type signature.
	/// </summary>
	/// <param name="filter">The filter.</param>
	public void Register(Filter filter)
	{
		Index.RegisterFilter(filter);
	}

	/// <summary>
	/// Enables an index for a component value.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	public void Register<T>() where T : struct
	{
		Index.RegisterComponentType<T>();
	}

	/// <summary>
	/// Retrieves the set of entities with a matching component structure.
	/// </summary>
	/// <param name="filter">The filter.</param>
	/// <returns>The entity set.</returns>
	public View View(Filter filter)
	{
		// TODO: Handle non-indexed
		return new(this, Index.GetEntities(filter));
	}

	/// <summary>
	/// Retrieves the set of entities with a matching component value.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	/// <param name="value">The component value.</param>
	/// <returns>The entity set.</returns>
	public View View<T>(T value) where T : struct
	{
		// TODO: Handle non-indexed
		return new(this, Index.GetEntities(value));
	}
}
