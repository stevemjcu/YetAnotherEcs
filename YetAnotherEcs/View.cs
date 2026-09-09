using System.Collections;
using YetAnotherEcs.Utility;

namespace YetAnotherEcs;

/// <summary>
/// The set of entities returned by a query.
/// </summary>
public readonly struct View : IEnumerable<Entity>, IReadOnlyCollection<Entity>
{
	private readonly World World;
	private readonly SparseSet Entities;

	internal View(World world, SparseSet ids)
	{
		World = world;
		Entities = ids;
	}

	public readonly int Count => Entities.Count;

	public readonly Entity this[int index] => new(World, Entities[index]);

	public readonly bool Contains(Entity entity)
	{
		return Entities.Contains(entity.Id);
	}

	public readonly ViewEnumerator GetEnumerator()
	{
		return new(World, Entities);
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
