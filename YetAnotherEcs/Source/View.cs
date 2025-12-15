using YetAnotherEcs.General;

namespace YetAnotherEcs;

public struct View(World World, SparseSet Set)
{
	public readonly int Count => Set.Count;

	public readonly Entity this[int item] => World.Get(Set[item]);

	public readonly bool Contains(Entity entity)
	{
		return Set.Contains(entity.Id);
	}

	public readonly ReverseEnumerator GetEnumerator()
	{
		return new(World, Set);
	}

	public struct ReverseEnumerator(World World, SparseSet Set)
	{
		private int Index = Set.Count;

		public readonly Entity Current => World.Get(Set[Index]);

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
			Index = Set.Count;
		}
	}
}
