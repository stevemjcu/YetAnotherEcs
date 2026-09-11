using YetAnotherEcs.Utility;

namespace YetAnotherEcs.Storage;

internal class Table
{
	private readonly IdPool EntityIdPool = new();
	private readonly (int Bitmask, int Version)[] EntityInfoById = new (int Bitmask, int Version)[1000]; // TODO: Parameterize
	private readonly Dictionary<int, object> ComponentStoreByTypeId = [];

	public IEnumerable<int> GetEntities()
	{
		for (var i = 0; i < EntityInfoById.Length; i++)
		{
			if (EntityInfoById[i].Bitmask > 0)
			{
				yield return i;
			}
		}
	}

	public int CreateEntity(out int version)
	{
		var id = EntityIdPool.Assign();
		version = EntityInfoById[id].Version++;
		return id;
	}

	public void DeleteEntity(int id)
	{
		EntityInfoById[id].Bitmask = 0;
		EntityIdPool.Recycle(id);
	}

	public int GetBitmask(int id)
	{
		return EntityInfoById[id].Bitmask;
	}

	public int GetVersion(int id)
	{
		return EntityInfoById[id].Version;
	}

	public bool HasComponent<T>(int id) where T : struct
	{
		return (EntityInfoById[id].Bitmask & ComponentType<T>.Bitmask) > 0;
	}

	public T GetComponent<T>(int id) where T : struct
	{
		return GetComponentStore<T>()[id];
	}

	public void SetComponent<T>(int id, T value = default) where T : struct
	{
		EntityInfoById[id].Bitmask |= ComponentType<T>.Bitmask;
		GetComponentStore<T>()[id] = value;
	}

	public void RemoveComponent<T>(int id) where T : struct
	{
		EntityInfoById[id].Bitmask &= ~ComponentType<T>.Bitmask;
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
