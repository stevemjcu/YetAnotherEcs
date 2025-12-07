using System.Runtime.InteropServices;
using YetAnotherEcs.General;

namespace YetAnotherEcs.Storage;

internal class Registry
{
	public event Action<int, int>? StructureChanged;
	public event Action<int, int>? ValueAdded;
	public event Action<int, int>? ValueRemoved;
	public event Action<int>? EntityRecycled;

	private readonly IdPool IdPool = new();
	private readonly List<int> BitmaskById = [];
	private readonly Dictionary<int, object> StorageByType = [];

	public int FlagBitmask;

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

	public void Flag<T>() where T : struct
	{
		FlagBitmask |= GetBitmask<T>();
	}

	public bool IsFlagged<T>() where T : struct
	{
		return (FlagBitmask & GetBitmask<T>()) > 0;
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
		EntityRecycled?.Invoke(id);
		IdPool.Recycle(id);
	}

	public void Set<T>(int id, T value) where T : struct
	{
		var store = GetComponentById<T>();

		if (IsFlagged<T>())
		{
			if (Has<T>(id))
			{
				ValueRemoved?.Invoke(id, Hash(store[id]));
			}

			ValueAdded?.Invoke(id, Hash(value));
		}

		if (!Has<T>(id))
		{
			BitmaskById[id] |= GetBitmask<T>();
			StructureChanged?.Invoke(id, BitmaskById[id]);
		}

		store[id] = value;
	}

	public void Remove<T>(int id) where T : struct
	{
		if (IsFlagged<T>())
		{
			ValueRemoved?.Invoke(id, Hash(GetComponentById<T>()[id]));
		}

		BitmaskById[id] ^= GetBitmask<T>();
		StructureChanged?.Invoke(id, BitmaskById[id]);

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
