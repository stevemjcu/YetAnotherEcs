using System.Runtime.InteropServices;
using YetAnotherEcs.General;

namespace YetAnotherEcs.Alt.Storage;

internal class EntityPool
{
	public event Action<int, int>? BitmaskChanged;
	public event Action<int, int, int, int>? IndexChanged;
	private int IndexBitmask;

	private readonly IdPool IdPool = new();
	private readonly List<int> BitmaskById = [];
	private readonly Dictionary<int, object> StorageByType = [];

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

	private static int TypeId<T>() where T : struct => TypedIdPool<EntityPool, T>.Id;

	private static int TypeBitmask<T>() where T : struct => 1 << TypeId<T>();

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
		BitmaskChanged?.Invoke(id, 0);
	}

	public bool Exists(int id) => id < BitmaskById.Count && BitmaskById[id] > 0;

	// TODO: Check for index change
	public void Set<T>(int id, T value) where T : struct
	{
		ComponentById<T>()[id] = value;

		var a = BitmaskById[id];
		var b = a | TypeBitmask<T>();

		BitmaskById[id] |= TypeBitmask<T>();
		if (a != b) BitmaskChanged?.Invoke(id, b);
	}

	public void Remove<T>(int id) where T : struct
	{
		ComponentById<T>()[id] = default;

		BitmaskById[id] ^= TypeBitmask<T>();
		BitmaskChanged?.Invoke(id, BitmaskById[id]);
	}

	public bool Has<T>(int id) where T : struct => (BitmaskById[id] & TypeBitmask<T>()) > 0;

	public T Get<T>(int id) where T : struct => ComponentById<T>()[id];
}
