using System.Collections;
using System.Runtime.InteropServices;
using YetAnotherEcs.Source.General;

namespace YetAnotherEcs.General;

public class SparseSet : IEnumerable<int>
{
	private readonly List<int> IndexByItem = []; // sparse
	private readonly List<int> ItemByIndex = []; // dense

	public int Count => ItemByIndex.Count;

	public int this[int item] => ItemByIndex[item];

	public bool Contains(int item)
	{
		return
			item < IndexByItem.Count &&
			IndexByItem[item] < ItemByIndex.Count &&
			item == ItemByIndex[IndexByItem[item]];
	}

	public bool Any()
	{
		return Count > 0;
	}

	public int Single()
	{
		return this[0];
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

	public void Remove(int item)
	{
		if (!Contains(item))
		{
			return;
		}

		var index0 = IndexByItem[item];
		var index1 = ItemByIndex.Count - 1;

		IndexByItem[item] = index1;
		IndexByItem[ItemByIndex[index1]] = index0;
		ItemByIndex[index0] = ItemByIndex[index1];

		ItemByIndex.RemoveAt(index1);
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

	IEnumerator<int> IEnumerable<int>.GetEnumerator()
	{
		return ItemByIndex.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return ItemByIndex.GetEnumerator();
	}
}
