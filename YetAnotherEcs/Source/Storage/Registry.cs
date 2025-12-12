using System.Runtime.InteropServices;
using YetAnotherEcs.General;

namespace YetAnotherEcs.Storage;

internal class Registry(World World)
{
	private readonly IdPool IdPool = new();
	internal readonly List<int> BitmaskById = [];
	private readonly List<int> VersionById = [];
	private readonly Dictionary<int, object> ComponentStoreByType = [];

	private Manifest Manifest => World.Manifest;

	public IEnumerable<Entity> GetEntities()
	{
		for (var i = 0; i < BitmaskById.Count; i++)
		{
			if (BitmaskById[i] > 0)
			{
				yield return new(i, BitmaskById[i], World.Id);
			}
		}
	}

	public Entity Create()
	{
		var id = IdPool.Assign();

		if (BitmaskById.Count < id + 1)
		{
			CollectionsMarshal.SetCount(BitmaskById, id + 1);
		}

		return new(id, ++VersionById[id], World.Id);
	}

	public void Destroy(int id)
	{
		BitmaskById[id] = 0;
		Manifest.OnEntityRecycled(id);
		IdPool.Recycle(id);
	}

	public void Set<T>(int id, T value) where T : struct
	{
		var store = GetComponentStore<T>();
		var has = TryGet<T>(id, out var last);

		if (Component<T>.Indexed)
		{
			if (has)
			{
				Manifest.OnIndexRemoved(id, last);
			}

			Manifest.OnIndexAdded(id, value);
		}

		if (!has)
		{
			BitmaskById[id] |= Component<T>.Bitmask;
			Manifest.OnStructureChanged(id, BitmaskById[id]);
		}

		store[id] = value;
	}

	public void Remove<T>(int id) where T : struct
	{
		var store = GetComponentStore<T>();

		if (Component<T>.Indexed)
		{
			Manifest.OnIndexRemoved(id, store[id]);
		}

		BitmaskById[id] &= ~Component<T>.Bitmask;
		Manifest.OnStructureChanged(id, BitmaskById[id]);

		store[id] = default;
	}

	public bool Has<T>(int id) where T : struct
	{
		return (BitmaskById[id] & Component<T>.Bitmask) > 0;
	}

	public T Get<T>(int id) where T : struct
	{
		return GetComponentStore<T>()[id];
	}

	public bool TryGet<T>(int id, out T value) where T : struct
	{
		var has = Has<T>(id);
		value = has ? Get<T>(id) : default;
		return has;
	}

	private Dictionary<int, T> GetComponentStore<T>() where T : struct
	{
		var typeId = Component<T>.Id;

		if (!ComponentStoreByType.TryGetValue(typeId, out var value))
		{
			value = new Dictionary<int, T>();
			ComponentStoreByType.Add(typeId, value);
		}

		// Maps entity to component
		return (Dictionary<int, T>)value;
	}
}
