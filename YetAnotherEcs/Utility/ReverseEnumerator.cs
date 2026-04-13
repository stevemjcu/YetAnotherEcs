using YetAnotherEcs.Utility;

namespace YetAnotherEcs.Source.General;

public struct ReverseEnumerator(SparseSet Set)
{
	private int Index = Set.Count;

	public readonly int Current => Set[Index];

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
