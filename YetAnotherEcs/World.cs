using YetAnotherEcs.Storage;
using Index = YetAnotherEcs.Storage.Index;

namespace YetAnotherEcs;

/// <summary>
/// The storage for entities and their components.
/// </summary>
public class World
{
	private readonly Database Database = new();
	private readonly Index Index = new();

	/// <summary>
	/// Creates an entity with a unique ID.
	/// </summary>
	/// <returns>The entity.</returns>
	public Entity Create()
	{
		return new(this, Database.CreateEntity(out var version), version);
	}

	/// <summary>
	/// Retrieves an entity.
	/// </summary>
	/// <param name="id">The entity id.</param>
	/// <returns>The entity.</returns>
	public Entity Get(int id)
	{
		return new(this, id, Database.GetVersion(id));
	}

	/// <summary>
	/// Deletes an entity and recycles its ID.
	/// </summary>
	/// <param name="entity">The entity.</param>
	public void Delete(Entity entity)
	{
		Database.DeleteEntity(entity.Id);
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
	/// <returns>The entity set.</returns>
	public View Query(Filter filter)
	{
		if (!Index.HasFilter(filter))
		{
			Index.RegisterFilter(filter);
			foreach (var id in Database.GetEntities())
			{
				Index.OnStructureChanged(id, Database.GetBitmask(id));
			}
		}

		return new(this, Index.GetEntities(filter));
	}

	/// <summary>
	/// Retrieves the set of entities with a matching component value.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	/// <param name="value">The component value.</param>
	/// <returns>The entity set.</returns>
	public View Query<T>(T value) where T : struct
	{
		if (!Index.HasComponentType<T>())
		{
			Index.RegisterComponentType<T>();
			foreach (var id in Database.GetEntities())
			{
				Index.OnComponentAdded(id, Database.GetComponent<T>(id));
			}
		}

		return new(this, Index.GetEntities(value));
	}

	internal bool HasComponent<T>(int id) where T : struct
	{
		return Database.HasComponent<T>(id);
	}

	internal T GetComponent<T>(int id) where T : struct
	{
		return Database.GetComponent<T>(id);
	}

	internal bool TryGetComponent<T>(int id, out T value) where T : struct
	{
		var exists = HasComponent<T>(id);
		value = exists ? GetComponent<T>(id) : default;
		return exists;
	}

	internal void SetComponent<T>(int id, T value = default) where T : struct
	{
		var exists = HasComponent<T>(id);

		if (Index.HasComponentType<T>())
		{
			if (exists)
			{
				Index.OnComponentRemoved(id, GetComponent<T>(id));
			}

			Index.OnComponentAdded(id, value);
		}

		Database.SetComponent(id, value);

		if (!exists)
		{
			Index.OnStructureChanged(id, Database.GetBitmask(id));
		}
	}

	internal void RemoveComponent<T>(int id) where T : struct
	{
		if (Index.HasComponentType<T>())
		{
			Index.OnComponentRemoved(id, GetComponent<T>(id));
		}

		Database.RemoveComponent<T>(id);
		Index.OnStructureChanged(id, Database.GetBitmask(id));
	}
}
