using YetAnotherEcs.Storage;
using Index = YetAnotherEcs.Storage.Index;

namespace YetAnotherEcs;

/// <summary>
/// Represents the storage for the entity component system.
/// </summary>
public class World
{
	private readonly Table Table = new();
	private readonly Index Index = new();

	/// <summary>
	/// Creates an entity with a unique ID.
	/// </summary>
	/// <returns>The entity.</returns>
	public Entity Create()
	{
		return new Entity(this, Table.CreateEntity());
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
	/// <returns>The entity set.</returns>
	public View View(Filter filter)
	{
		if (!Index.ContainsFilter(filter))
		{
			Index.RegisterFilter(filter);
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
	/// <returns>The entity set.</returns>
	public View View<T>(T value) where T : struct
	{
		if (!Index.ContainsComponentType<T>())
		{
			Index.RegisterComponentType<T>();
			foreach (var (id, _) in Table.GetEntities())
			{
				Index.OnComponentAdded(id, Table.GetComponent<T>(id));
			}
		}

		return new(this, Index.GetEntities(value));
	}

	internal bool Has<T>(int id) where T : struct
	{
		return Table.HasComponent<T>(id);
	}

	internal T Get<T>(int id) where T : struct
	{
		return Table.GetComponent<T>(id);
	}

	internal bool TryGet<T>(int id, out T value) where T : struct
	{
		var exists = Has<T>(id);
		value = exists ? Get<T>(id) : default;
		return exists;
	}

	internal void Set<T>(int id, T value = default) where T : struct
	{
		var exists = Has<T>(id);

		if (Index.ContainsComponentType<T>())
		{
			if (exists)
			{
				Index.OnComponentRemoved(id, Get<T>(id));
			}

			Index.OnComponentAdded(id, value);
		}

		Table.SetComponent(id, value);

		if (!exists)
		{
			Index.OnStructureChanged(id, Table.GetBitmask(id));
		}
	}

	internal void Remove<T>(int id) where T : struct
	{
		if (Index.ContainsComponentType<T>())
		{
			Index.OnComponentRemoved(id, Get<T>(id));
		}

		Table.RemoveComponent<T>(id);
		Index.OnStructureChanged(id, Table.GetBitmask(id));
	}
}
