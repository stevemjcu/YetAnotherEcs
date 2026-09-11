using YetAnotherEcs.Utility;
using static YetAnotherEcs.Utility.Extensions;

namespace YetAnotherEcs.Storage;

internal class Database
{
	private record struct EntityInfo(int Version, int Bitmask);

	private readonly IdPool EntityIdPool = new();
	private readonly List<EntityInfo> EntityInfoById = [];
	private readonly Dictionary<int, object> ComponentStoreByTypeId = [];

	public IEnumerable<int> GetEntities()
	{
		for (var i = 0; i < EntityInfoById.Count; i++)
		{
			if (EntityInfoById[i].Bitmask > 0)
			{
				yield return i;
			}
		}
	}

	public (int Id, int Version) CreateEntity()
	{
		var id = EntityIdPool.Assign();
		EntityInfoById.EnsureCount(id + 1);
		return (id, EntityInfoById[id].Version);
	}

	public void DeleteEntity(int id)
	{
		ref var info = ref EntityInfoById.AsSpan()[id];
		info.Bitmask = 0;
		info.Version++;
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
		EntityInfoById.AsSpan()[id].Bitmask |= ComponentType<T>.Bitmask;
		var store = GetComponentStore<T>();
		store.EnsureCount(id + 1);
		store[id] = value;
	}

	public void RemoveComponent<T>(int id) where T : struct
	{
		EntityInfoById.AsSpan()[id].Bitmask &= ~ComponentType<T>.Bitmask;
		GetComponentStore<T>()[id] = default;
	}

	private List<T> GetComponentStore<T>() where T : struct
	{
		var typeId = ComponentType<T>.Id;

		if (!ComponentStoreByTypeId.TryGetValue(typeId, out var value))
		{
			value = new List<T>();
			ComponentStoreByTypeId.Add(typeId, value);
		}

		return (List<T>)value;
	}
}
