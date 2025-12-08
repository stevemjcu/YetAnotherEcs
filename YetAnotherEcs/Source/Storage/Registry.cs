using System.Runtime.InteropServices;
using YetAnotherEcs.General;

namespace YetAnotherEcs.Storage;

internal class Registry(Index Index)
{
	private readonly IdPool IdPool = new();
	private readonly List<int> BitmaskById = [];
	private readonly Dictionary<int, object> StorageByType = [];

	public int IndexBitmask;

	internal IEnumerable<(int, int)> Enumerate()
	{
		for (var i = 0; i < BitmaskById.Count; i++)
		{
			var bitmask = BitmaskById[i];

			if (bitmask > 0)
			{
				yield return (i, bitmask);
			}
		}
	}

	private Dictionary<int, T> GetComponentById<T>() where T : struct
	{
		var typeId = GetId<T>();

		if (!StorageByType.TryGetValue(typeId, out var value))
		{
			value = new Dictionary<int, T>();
			StorageByType.Add(typeId, value);
		}

		// TODO: Use sparse dictionary?
		return (Dictionary<int, T>)value;
	}

	public static int GetId<T>() where T : struct
	{
		return TypedIdPool<Registry, T>.Id;
	}

	public static int GetBitmask<T>() where T : struct
	{
		return 1 << GetId<T>();
	}

	public static int Hash<T>(T value) where T : struct
	{
		return (GetId<T>(), value).GetHashCode();
	}

	public void IndexBy<T>() where T : struct
	{
		IndexBitmask |= GetBitmask<T>();
	}

	public bool IsIndexed<T>() where T : struct
	{
		return (IndexBitmask & GetBitmask<T>()) > 0;
	}

	public int Create()
	{
		var id = IdPool.Assign();

		if (BitmaskById.Count < id + 1)
		{
			CollectionsMarshal.SetCount(BitmaskById, id + 1);
		}

		return id;
	}

	public void Recycle(int id)
	{
		BitmaskById[id] = 0;
		Index.OnEntityRecycled(id);
		IdPool.Recycle(id);
	}

	public void Set<T>(int id, T value) where T : struct
	{
		var store = GetComponentById<T>();

		if (IsIndexed<T>())
		{
			if (Has<T>(id))
			{
				Index.OnIndexRemoved(id, Hash(store[id]));
			}

			Index.OnIndexAdded(id, Hash(value));
		}

		if (!Has<T>(id))
		{
			BitmaskById[id] |= GetBitmask<T>();
			Index.OnStructureChanged(id, BitmaskById[id]);
		}

		store[id] = value;
	}

	public void Remove<T>(int id) where T : struct
	{
		if (IsIndexed<T>())
		{
			Index.OnIndexRemoved(id, Hash(GetComponentById<T>()[id]));
		}

		BitmaskById[id] ^= GetBitmask<T>();
		Index.OnStructureChanged(id, BitmaskById[id]);

		GetComponentById<T>()[id] = default;
	}

	public bool Has<T>(int id) where T : struct
	{
		return (BitmaskById[id] & GetBitmask<T>()) > 0;
	}

	public T Get<T>(int id) where T : struct
	{
		return GetComponentById<T>()[id];
	}
}
