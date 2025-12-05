using System.Runtime.InteropServices;
using YetAnotherEcs.General;

namespace YetAnotherEcs.Storage;

internal class EntityStore
{
	public event Action<int, int>? BitmaskChanged;
	public event Action<int, int, int>? IndexChanged;
	public event Action<int>? EntityDestroyed;

	private readonly IdPool IdPool = new();
	private readonly List<int> BitmaskById = [];
	private readonly Dictionary<int, object> StorageByType = [];

	public int IndexBitmask;

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

	public static int TypeId<T>() where T : struct => TypedIdPool<EntityStore, T>.Id;

	public static int TypeBitmask<T>() where T : struct => 1 << TypeId<T>();

	public static int Hash<T>(T value) where T : struct => (TypeId<T>(), value).GetHashCode();

	public void Index<T>() where T : struct => IndexBitmask |= TypeBitmask<T>();

	// TODO: Compare type bitmask to index bitmask
	private bool IsIndexed<T>() where T : struct => false;

	public int Create()
	{
		var id = IdPool.Assign();

		if (BitmaskById.Count < id + 1)
			CollectionsMarshal.SetCount(BitmaskById, id + 1);

		return id;
	}

	public void Destroy(int id)
	{
		BitmaskById[id] = 0;
		EntityDestroyed?.Invoke(id);
	}

	public bool Exists(int id) => id < BitmaskById.Count && BitmaskById[id] > 0;

	public void Set<T>(int id, T value) where T : struct
	{
		var bitmask = TypeBitmask<T>();
		var store = ComponentById<T>();

		if (IsIndexed<T>())
		{
			store.TryGetValue(id, out var prev);
			IndexChanged?.Invoke(id, Hash(prev), Hash(value));
		}

		store[id] = value;

		var a = BitmaskById[id];
		var b = a | bitmask;

		BitmaskById[id] |= bitmask;
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
