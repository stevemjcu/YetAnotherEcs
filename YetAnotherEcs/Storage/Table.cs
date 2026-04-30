using System.Runtime.InteropServices;
using YetAnotherEcs.Utility;

namespace YetAnotherEcs.Storage;

internal class Table
{
	private readonly IdPool EntityIdPool = new();
	private readonly List<int> BitmaskByEntityId = [];
	private readonly Dictionary<int, object> ComponentStoreByTypeId = [];

	public IEnumerable<(int, int)> GetEntities()
	{
		for (var i = 0; i < BitmaskByEntityId.Count; i++)
		{
			if (BitmaskByEntityId[i] > 0)
			{
				yield return (i, BitmaskByEntityId[i]);
			}
		}
	}

	public int CreateEntity()
	{
		var id = EntityIdPool.Assign();

		if (BitmaskByEntityId.Count < id + 1)
		{
			CollectionsMarshal.SetCount(BitmaskByEntityId, id + 1);
		}

		return id;
	}

	public void DeleteEntity(int id)
	{
		BitmaskByEntityId[id] = 0;
		EntityIdPool.Recycle(id);
	}

	public int GetBitmask(int id)
	{
		return BitmaskByEntityId[id];
	}

	public bool HasComponent<T>(int id) where T : struct
	{
		return (BitmaskByEntityId[id] & ComponentType<T>.Bitmask) > 0;
	}

	public T GetComponent<T>(int id) where T : struct
	{
		return GetComponentStore<T>()[id];
	}

	public void SetComponent<T>(int id, T value = default) where T : struct
	{
		BitmaskByEntityId[id] |= ComponentType<T>.Bitmask;
		GetComponentStore<T>()[id] = value;
	}

	public void RemoveComponent<T>(int id) where T : struct
	{
		BitmaskByEntityId[id] &= ~ComponentType<T>.Bitmask;
		GetComponentStore<T>()[id] = default;
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
}
