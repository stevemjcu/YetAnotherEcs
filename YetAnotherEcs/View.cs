using System.Collections;
using YetAnotherEcs.Utility;

namespace YetAnotherEcs;

/// <summary>
/// The set of entities returned by a query.
/// </summary>
public readonly struct View : IEnumerable<Entity>, IReadOnlyCollection<Entity>
{
	private readonly World World;
	private readonly SparseSet Ids;

	internal View(World world, SparseSet ids)
	{
		World = world;
		Ids = ids;
	}

	public readonly int Count => Ids.Count;

	public readonly Entity this[int index] => World.Get(Ids[index]);

	public readonly bool Contains(Entity entity)
	{
		return Ids.Contains(entity.Id);
	}

	public readonly ViewEnumerator GetEnumerator()
	{
		return new(World, Ids);
	}

	IEnumerator<Entity> IEnumerable<Entity>.GetEnumerator()
	{
		return GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
