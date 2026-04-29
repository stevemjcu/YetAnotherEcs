using System.Collections;
using System.Runtime.InteropServices;
using YetAnotherEcs.Utility;

namespace YetAnotherEcs;

/// <summary>
/// Manages a collection of entities and their components.
/// </summary>
public class World
{
	private readonly IdPool EntityIdPool = new();
	private readonly List<int> BitmaskByEntityId = [];
	private readonly Dictionary<int, object> ComponentStoreByTypeId = [];

	private static readonly SparseSet EmptySet = [];
	private readonly Dictionary<Filter, SparseSet> EntityIdSetByFilter = [];
	private readonly Dictionary<int, object> IndexStoreByTypeId = [];

	#region Entities

	/// <summary>
	/// Creates an entity with a unique ID.
	/// </summary>
	/// <returns>The entity ID.</returns>
	public int CreateEntity()
	{
		var id = EntityIdPool.Assign();

		if (BitmaskByEntityId.Count < id + 1)
		{
			CollectionsMarshal.SetCount(BitmaskByEntityId, id + 1);
		}

		return id;
	}

	/// <summary>
	/// Deletes an entity and recycles its ID.
	/// </summary>
	/// <param name="id">The entity ID.</param>
	public void DeleteEntity(int id)
	{
		BitmaskByEntityId[id] = 0;
		OnEntityDeleted(id);
		EntityIdPool.Recycle(id);
	}

	#endregion

	#region Components

	/// <summary>
	/// Checks if an entity has a component.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	/// <param name="id">The entity ID.</param>
	/// <returns>True if the component exists; otherwise, false.</returns>
	public bool HasComponent<T>(int id) where T : struct
	{
		return (BitmaskByEntityId[id] & ComponentType<T>.Bitmask) > 0;
	}

	/// <summary>
	/// Gets the component associated with an entity.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	/// <param name="id">The entity ID.</param>
	/// <returns>The component.</returns>
	public T GetComponent<T>(int id) where T : struct
	{
		return GetComponentStore<T>()[id];
	}

	/// <summary>
	/// Gets the component associated with an entity, if it exists.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	/// <param name="id">The entity ID.</param>
	/// <param name="value">The component.</param>
	/// <returns>True if the component exists; otherwise, false.</returns>
	public bool TryGetComponent<T>(int id, out T value) where T : struct
	{
		var has = HasComponent<T>(id);
		value = has ? GetComponent<T>(id) : default;
		return has;
	}

	/// <summary>
	/// Sets the component associated with an entity.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	/// <param name="id">The entity ID.</param>
	/// <param name="value">The component.</param>
	public void SetComponent<T>(int id, T value = default) where T : struct
	{
		var store = GetComponentStore<T>();
		var exists = HasComponent<T>(id);

		if (ComponentType<T>.Indexed)
		{
			if (exists)
			{
				OnIndexRemoved(id, store[id]);
			}

			OnIndexAdded(id, value);
		}

		if (!exists)
		{
			BitmaskByEntityId[id] |= ComponentType<T>.Bitmask;
			OnStructureChanged(id, BitmaskByEntityId[id]);
		}

		store[id] = value;
	}

	/// <summary>
	/// Removes the component associated with an entity.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	/// <param name="id">The entity ID.</param>
	public void RemoveComponent<T>(int id) where T : struct
	{
		var store = GetComponentStore<T>();

		if (ComponentType<T>.Indexed)
		{
			OnIndexRemoved(id, store[id]);
		}

		BitmaskByEntityId[id] &= ~ComponentType<T>.Bitmask;
		OnStructureChanged(id, BitmaskByEntityId[id]);

		store[id] = default;
	}

	private Dictionary<int, T> GetComponentStore<T>() where T : struct
	{
		var typeId = ComponentType<T>.Id;

		if (!ComponentStoreByTypeId.TryGetValue(typeId, out var value))
		{
			value = new Dictionary<int, T>();
			ComponentStoreByTypeId.Add(typeId, value);
		}

		// Maps component by entity ID
		return (Dictionary<int, T>)value;
	}

	#endregion

	#region Queries

	/// <summary>
	/// Gets the entity IDs matching an filter.
	/// </summary>
	/// <param name="filter">The filter.</param>
	/// <returns>The entity ID set.</returns>
	public SparseSet QueryEntities(Filter filter)
	{
		if (!EntityIdSetByFilter.TryGetValue(filter, out var value))
		{
			value = [];
			EntityIdSetByFilter[filter] = value;
		}

		return value;
	}

	/// <summary>
	/// Gets the entity IDs matching an index.
	/// </summary>
	/// <typeparam name="T">The index component type.</typeparam>
	/// <param name="index">The index component.</param>
	/// <returns>The entity ID set.</returns>
	public SparseSet QueryEntities<T>(T index) where T : struct
	{
		return GetIndexStore<T>().TryGetValue(index, out var set) ? set : EmptySet;
	}

	private void OnStructureChanged(int id, int bitmask)
	{
		foreach (var it in EntityIdSetByFilter)
		{
			if (it.Key.Matches(bitmask))
			{
				it.Value.Add(id);
			}
			else
			{
				it.Value.Remove(id);
			}
		}
	}

	private void OnIndexAdded<T>(int id, T index) where T : struct
	{
		var store = GetIndexStore<T>();

		if (!store.TryGetValue(index, out var set))
		{
			store[index] = set = [];
		}

		set.Add(id);
	}

	private void OnIndexRemoved<T>(int id, T index) where T : struct
	{
		var store = GetIndexStore<T>();

		if (store.TryGetValue(index, out var set))
		{
			set.Remove(id);
		}
	}

	private void OnEntityDeleted(int id)
	{
		foreach (var it in EntityIdSetByFilter.Values)
		{
			it.Remove(id);
		}

		foreach (var store in IndexStoreByTypeId.Values.Cast<IDictionary>())
		{
			foreach (SparseSet it in store.Values)
			{
				it.Remove(id);
			}
		}
	}

	private Dictionary<T, SparseSet> GetIndexStore<T>() where T : struct
	{
		var typeId = ComponentType<T>.Id;

		if (!IndexStoreByTypeId.TryGetValue(typeId, out var value))
		{
			value = new Dictionary<T, SparseSet>();
			IndexStoreByTypeId.Add(typeId, value);
		}

		// Maps entity ID set by component
		return (Dictionary<T, SparseSet>)value;
	}

	#endregion
}
