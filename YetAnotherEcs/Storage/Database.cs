using YetAnotherEcs.Utility;
using static YetAnotherEcs.Utility.Extensions;

namespace YetAnotherEcs.Storage;

internal class Database
{
	private record struct EntityInfo(int Version, int Bitmask);

	private readonly IdPool EntityIdPool = new();
	private readonly List<EntityInfo> EntityInfoList = [];
	private readonly Dictionary<int, object> ComponentStoreByTypeId = [];

	private Span<EntityInfo> EntityInfoSpan => EntityInfoList.AsSpan();

	public IEnumerable<int> GetEntities()
	{
		for (var i = 0; i < EntityInfoSpan.Length; i++)
		{
			if (EntityInfoSpan[i].Bitmask > 0)
			{
				yield return i;
			}
		}
	}

	public (int Id, int Version) CreateEntity()
	{
		var id = EntityIdPool.Assign();
		EntityInfoList.EnsureCount(id + 1);
		return (id, EntityInfoSpan[id].Version);
	}

	public void DeleteEntity(int id)
	{
		ref var info = ref EntityInfoSpan[id];
		info.Bitmask = 0;
		info.Version++;
		EntityIdPool.Recycle(id);
	}

	public int GetBitmask(int id)
	{
		return EntityInfoSpan[id].Bitmask;
	}

	public int GetVersion(int id)
	{
		return EntityInfoSpan[id].Version;
	}

	public bool HasComponent<T>(int id) where T : struct
	{
		return (EntityInfoSpan[id].Bitmask & ComponentType<T>.Bitmask) > 0;
	}

	public T GetComponent<T>(int id) where T : struct
	{
		return GetComponentStore<T>()[id];
	}

	public void SetComponent<T>(int id, T value = default) where T : struct
	{
		EntityInfoSpan[id].Bitmask |= ComponentType<T>.Bitmask;
		var store = GetComponentStore<T>();
		store.EnsureCount(id + 1);
		store[id] = value;
	}

	public void RemoveComponent<T>(int id) where T : struct
	{
		EntityInfoSpan[id].Bitmask &= ~ComponentType<T>.Bitmask;
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
