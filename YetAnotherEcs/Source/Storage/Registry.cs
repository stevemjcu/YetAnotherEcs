using System.Runtime.InteropServices;
using YetAnotherEcs.General;

namespace YetAnotherEcs.Storage;

internal class Registry
{
	public event Action<int, int>? StructureChanged;
	public event Action<int, int>? ValueAdded;
	public event Action<int, int>? ValueRemoved;
	public event Action<int>? EntityDestroyed;

	private readonly IdPool IdPool = new();
	private readonly List<int> BitmaskById = [];
	private readonly Dictionary<int, object> StorageByType = [];

	public int FlagBitmask;

	// TODO: Replace with sparse dictionary
	private Dictionary<int, T> ComponentById<T>() where T : struct
	{
		var typeId = TypeId<T>();

		if (!StorageByType.TryGetValue(typeId, out var value))
		{
			value = new Dictionary<int, T>();
			StorageByType.Add(typeId, value);
		}

		return (Dictionary<int, T>)value;
	}

	public static int TypeId<T>() where T : struct => TypedIdPool<Registry, T>.Id;

	public static int TypeBitmask<T>() where T : struct => 1 << TypeId<T>();

	public static int Hash<T>(T value) where T : struct => (TypeId<T>(), value).GetHashCode();

	public void Flag<T>() where T : struct => FlagBitmask |= TypeBitmask<T>();

	public bool IsFlagged<T>() where T : struct => (FlagBitmask & TypeBitmask<T>()) > 0;

	public int Create()
	{
		var id = IdPool.Assign();

		if (BitmaskById.Count < id + 1)
		{
			CollectionsMarshal.SetCount(BitmaskById, id + 1);
		}

		return id;
	}

	public void Destroy(int id)
	{
		BitmaskById[id] = 0;
		EntityDestroyed?.Invoke(id);
	}

	public bool IsAlive(int id) => id < BitmaskById.Count && BitmaskById[id] > 0;

	public void Set<T>(int id, T value) where T : struct
	{
		var bitmask = TypeBitmask<T>();
		var store = ComponentById<T>();

		if (IsFlagged<T>())
		{
			if (store.TryGetValue(id, out var last))
			{
				ValueRemoved?.Invoke(id, Hash(last));
			}

			ValueAdded?.Invoke(id, Hash(value));
		}

		store[id] = value;

		var bitmask0 = BitmaskById[id];
		var bitmask1 = bitmask0 | bitmask;

		if (bitmask0 != bitmask1)
		{
			StructureChanged?.Invoke(id, bitmask1);
		}

		BitmaskById[id] |= bitmask;
	}

	public void Remove<T>(int id) where T : struct
	{
		BitmaskById[id] ^= TypeBitmask<T>();
		StructureChanged?.Invoke(id, BitmaskById[id]);

		if (IsFlagged<T>())
		{
			ValueRemoved?.Invoke(id, Hash(ComponentById<T>()[id]));
		}

		ComponentById<T>()[id] = default;
	}

	public bool Has<T>(int id) where T : struct => (BitmaskById[id] & TypeBitmask<T>()) > 0;

	public T Get<T>(int id) where T : struct => ComponentById<T>()[id];
}
