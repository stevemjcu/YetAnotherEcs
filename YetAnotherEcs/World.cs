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
	/// Retrieves the set of entities with a matching component structure.
	/// </summary>
	/// <param name="filter">The filter.</param>
	/// <returns>The view.</returns>
	public View View(Filter filter)
	{
		// TODO: Allow non-cached filters
		if (!Index.RegisterFilter(filter))
		{
			foreach (var (id, bitmask) in Table.GetEntities())
			{
				Index.OnStructureChanged(id, bitmask);
			}
		}

		return new(this, Index.GetEntities(filter));
	}

	/// <summary>
	/// Retrieves the set of entities with a matching component value.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	/// <param name="value">The component value.</param>
	/// <returns>The view.</returns>
	public View View<T>(T value) where T : struct
	{
		// TODO: Build if new or not indexed
		return new(this, Index.GetEntities(value));
	}
}
