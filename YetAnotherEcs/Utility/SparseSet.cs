using System.Collections;
using System.Runtime.InteropServices;

namespace YetAnotherEcs.Utility;

/// <summary>
/// Represents a set of non-negative integers stored contiguously.
/// </summary>
public class SparseSet : IEnumerable<int>, ICollection<int>
{
	private readonly List<int> IndexByItem = []; // sparse
	private readonly List<int> ItemByIndex = []; // dense

	public int Count => ItemByIndex.Count;

	public bool IsReadOnly => false;

	public int this[int index] => ItemByIndex[index];

	public bool Contains(int item)
	{
		return
			item < IndexByItem.Count &&
			IndexByItem[item] < ItemByIndex.Count &&
			item == ItemByIndex[IndexByItem[item]];
	}

	public void Add(int item)
	{
		if (Contains(item))
		{
			return;
		}

		if (item >= IndexByItem.Count)
		{
			CollectionsMarshal.SetCount(IndexByItem, item + 1);
		}

		IndexByItem[item] = ItemByIndex.Count;
		ItemByIndex.Add(item);
	}

	public bool Remove(int item)
	{
		if (!Contains(item))
		{
			return false;
		}

		var index0 = IndexByItem[item];
		var index1 = ItemByIndex.Count - 1;

		IndexByItem[item] = index1;
		IndexByItem[ItemByIndex[index1]] = index0;
		ItemByIndex[index0] = ItemByIndex[index1];

		ItemByIndex.RemoveAt(index1);
		return true;
	}

	public void Clear()
	{
		IndexByItem.Clear();
		ItemByIndex.Clear();
	}

	public ReverseEnumerator GetEnumerator()
	{
		return new(this);
	}

	/// <summary>
	/// Traverses the set in reverse to avoid invalidation.
	/// </summary>
	/// <param name="Set">The set to traverse.</param>
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

	IEnumerator<int> IEnumerable<int>.GetEnumerator()
	{
		return ItemByIndex.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return ItemByIndex.GetEnumerator();
	}

	public void CopyTo(int[] array, int arrayIndex)
	{
		throw new NotImplementedException();
	}
}
