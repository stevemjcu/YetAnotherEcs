using System.Runtime.InteropServices;

namespace YetAnotherEcs.General;

public class SparseSet : IIndexableSet<int>
{
	private readonly List<int> Sparse = []; // index by item
	private readonly List<int> Dense = []; // item by index

	public int Count => Dense.Count;

	public int this[int item] => Dense[item];

	public bool Contains(int item)
	{
		return
			item < Sparse.Count &&
			Sparse[item] < Dense.Count &&
			item == Dense[Sparse[item]];
	}

	public void Add(int item)
	{
		if (Contains(item))
		{
			return;
		}

		if (item >= Sparse.Count)
		{
			CollectionsMarshal.SetCount(Sparse, item + 1);
		}

		Sparse[item] = Dense.Count;
		Dense.Add(item);
	}

	public void Remove(int item)
	{
		if (!Contains(item))
		{
			return;
		}

		var index0 = Sparse[item];
		var index1 = Dense.Count - 1;

		Sparse[item] = index1;
		Sparse[Dense[index1]] = index0;
		Dense[index0] = Dense[index1];

		Dense.RemoveAt(index1);
	}

	public void Clear()
	{
		Sparse.Clear();
		Dense.Clear();
	}
}
