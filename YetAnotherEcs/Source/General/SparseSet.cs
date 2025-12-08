using System.Runtime.InteropServices;

namespace YetAnotherEcs.General;

public class SparseSet : IIndexableSet<int>
{
	private readonly List<int> Sparse = []; // index by element
	private readonly List<int> Dense = []; // element by index

	public int Count => Dense.Count;

	public int this[int element] => Dense[element];

	public void Add(int element)
	{
		if (Contains(element))
		{
			return;
		}

		if (element >= Sparse.Count)
		{
			CollectionsMarshal.SetCount(Sparse, element + 1);
		}

		Sparse[element] = Dense.Count;
		Dense.Add(element);
	}

	public bool Contains(int element)
	{
		return
			element < Sparse.Count &&
			Sparse[element] < Dense.Count &&
			element == Dense[Sparse[element]];
	}

	public void Remove(int element)
	{
		if (!Contains(element))
		{
			return;
		}

		var index0 = Sparse[element];
		var index1 = Dense.Count - 1;

		Sparse[element] = index1;
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
