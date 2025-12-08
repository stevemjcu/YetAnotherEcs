using System.Runtime.InteropServices;
using YetAnotherEcs.General;

namespace YetAnotherEcs.Storage;

internal class Registry(Index Index)
{
	private readonly IdPool IdPool = new();
	private readonly List<int> BitmaskById = [];
	private readonly Dictionary<int, object> ComponentStoreByType = [];

	private int IndexBitmask;

	private Dictionary<int, T> GetComponentStore<T>() where T : struct, IComponent
	{
		var typeId = IComponent.GetId<T>();

		if (!ComponentStoreByType.TryGetValue(typeId, out var value))
		{
			value = new Dictionary<int, T>();
			ComponentStoreByType.Add(typeId, value);
		}

		// TODO: Use sparse dictionary?
		return (Dictionary<int, T>)value;
	}

	public void IndexBy<T>() where T : struct, IComponent
	{
		IndexBitmask |= IComponent.GetBitmask<T>();
	}

	public bool IsIndexed<T>() where T : struct, IComponent
	{
		return (IndexBitmask & IComponent.GetBitmask<T>()) > 0;
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

	public void Set<T>(int id, T value) where T : struct, IComponent
	{
		var store = GetComponentStore<T>();
		var had = Has<T>(id);

		if (IsIndexed<T>())
		{
			if (had)
			{
				Index.OnIndexRemoved(id, IComponent.GetHashCode(store[id]));
			}

			Index.OnIndexAdded(id, IComponent.GetHashCode(value));
		}

		if (!had)
		{
			BitmaskById[id] |= IComponent.GetBitmask<T>();
			Index.OnStructureChanged(id, BitmaskById[id]);
		}

		store[id] = value;
	}

	public void Remove<T>(int id) where T : struct, IComponent
	{
		var store = GetComponentStore<T>();

		if (IsIndexed<T>())
		{
			Index.OnIndexRemoved(id, IComponent.GetHashCode(store[id]));
		}

		BitmaskById[id] ^= IComponent.GetBitmask<T>();
		Index.OnStructureChanged(id, BitmaskById[id]);

		store[id] = default;
	}

	public bool Has<T>(int id) where T : struct, IComponent
	{
		return (BitmaskById[id] & IComponent.GetBitmask<T>()) > 0;
	}

	public T Get<T>(int id) where T : struct, IComponent
	{
		return GetComponentStore<T>()[id];
	}
}
