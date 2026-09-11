using System.Collections;
using YetAnotherEcs.Utility;

namespace YetAnotherEcs;

/// <summary>
/// An enumerator for the set of entities returned by a query.
/// </summary>
public struct ViewEnumerator : IEnumerator<Entity>
{
	private readonly World World;
	private readonly SparseSet Ids;

	private int Index;

	public readonly Entity Current => World.Get(Ids[Index]);

	readonly object IEnumerator.Current => Current;

	internal ViewEnumerator(World world, SparseSet ids)
	{
		World = world;
		Ids = ids;
		Index = ids.Count;
	}

	public bool MoveNext()
	{
		while (--Index >= 0)
		{
			return true;
		}

		return false;
	}

	public void Reset()
	{
		Index = Ids.Count;
	}

	readonly void IDisposable.Dispose()
	{
	}
}
