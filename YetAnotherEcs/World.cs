using System.Collections;
using System.Runtime.InteropServices;
using YetAnotherEcs.Utility;

namespace YetAnotherEcs;

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
	/// Create an entity and return its ID.
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
	/// Delete the entity and recycle its ID.
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
	/// Check if the entity has the component.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	/// <param name="id">The entity ID.</param>
	/// <returns>True if the component exists; otherwise, false.</returns>
	public bool HasComponent<T>(int id) where T : struct
	{
		return (BitmaskByEntityId[id] & Component<T>.Bitmask) > 0;
	}

	/// <summary>
	/// Get the component associated with the entity.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	/// <param name="id">The entity ID.</param>
	/// <returns>The component.</returns>
	public T GetComponent<T>(int id) where T : struct
	{
		return GetComponentStore<T>()[id];
	}

	/// <summary>
	/// Set the component associated with the entity.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	/// <param name="id">The entity ID.</param>
	/// <param name="value">The component.</param>
	public void SetComponent<T>(int id, T value = default) where T : struct
	{
		var store = GetComponentStore<T>();
		var exists = HasComponent<T>(id);

		if (Component<T>.Indexed)
		{
			if (exists)
			{
				OnIndexRemoved(id, store[id]);
			}

			OnIndexAdded(id, value);
		}

		if (!exists)
		{
			BitmaskByEntityId[id] |= Component<T>.Bitmask;
			OnStructureChanged(id, BitmaskByEntityId[id]);
		}

		store[id] = value;
	}

	/// <summary>
	/// Remove the component associated with the entity.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	/// <param name="id">The entity ID.</param>
	public void RemoveComponent<T>(int id) where T : struct
	{
		var store = GetComponentStore<T>();

		if (Component<T>.Indexed)
		{
			OnIndexRemoved(id, store[id]);
		}

		BitmaskByEntityId[id] &= ~Component<T>.Bitmask;
		OnStructureChanged(id, BitmaskByEntityId[id]);

		store[id] = default;
	}

	private Dictionary<int, T> GetComponentStore<T>() where T : struct
	{
		var typeId = Component<T>.Id;

		if (!ComponentStoreByTypeId.TryGetValue(typeId, out var value))
		{
			value = new Dictionary<int, T>();
			ComponentStoreByTypeId.Add(typeId, value);
		}

		// Maps component by entity ID
		return (Dictionary<int, T>)value;
	}

	#endregion

	#region Views

	/// <summary>
	/// Get the entity IDs matching the filter.
	/// </summary>
	/// <param name="filter">The filter.</param>
	/// <returns>The entity ID set.</returns>
	public SparseSet GetView(Filter filter)
	{
		if (!EntityIdSetByFilter.TryGetValue(filter, out var value))
		{
			value = [];
			EntityIdSetByFilter[filter] = value;
		}

		return value;
	}

	/// <summary>
	/// Get the entity IDs matching the index.
	/// </summary>
	/// <typeparam name="T">The index component type.</typeparam>
	/// <param name="index">The index component.</param>
	/// <returns>The entity ID set.</returns>
	public SparseSet GetView<T>(T index) where T : struct
	{
		return GetIndexStore<T>().TryGetValue(index, out var set) ? set : EmptySet;
	}

	private void OnStructureChanged(int id, int bitmask)
	{
		foreach (var it in EntityIdSetByFilter)
		{
			if (it.Key.Compare(bitmask))
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
		var typeId = Component<T>.Id;

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
