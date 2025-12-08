using System.Runtime.InteropServices;

namespace YetAnotherEcs.General;

public class SparseSet : IIndexableSet<int>
{
	private readonly List<int> Sparse = []; // subkey by key
	private readonly List<int> Dense = []; // key by subkey

	public static readonly SparseSet Empty = [];

	public int Count => Dense.Count;

	public int this[int index] => Dense[index];

	public void Add(int key)
	{
		if (Contains(key))
		{
			return;
		}

		if (key >= Sparse.Count)
		{
			CollectionsMarshal.SetCount(Sparse, key + 1);
		}

		Sparse[key] = Dense.Count;
		Dense.Add(key);
	}

	public bool Contains(int key)
	{
		return key < Sparse.Count && key == Dense[Sparse[key]];
	}

	public void Remove(int key)
	{
		if (!Contains(key))
		{
			return;
		}

		var subkey = Sparse[key];
		var end = Dense.Count - 1;

		Sparse[key] = end;
		Sparse[Dense[end]] = subkey;
		Dense[subkey] = key;

		Dense.RemoveAt(end);
	}

	public void Clear()
	{
		Sparse.Clear();
		Dense.Clear();
	}
}
