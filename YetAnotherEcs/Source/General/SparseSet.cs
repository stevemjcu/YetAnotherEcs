using System.Runtime.InteropServices;

namespace YetAnotherEcs.General;

public class SparseSet
{
	private readonly List<int> Sparse = []; // subkey by key
	private readonly List<int> Dense = []; // key by subkey

	public int Count => Dense.Count;

	public void Add(int key)
	{
		if (key >= Sparse.Count)
		{
			CollectionsMarshal.SetCount(Sparse, key + 1);
		}

		Sparse[key] = Dense.Count;
		Dense.Add(key);
	}

	public bool Contains(int key)
	{
		return key == Dense[Sparse[key]];
	}

	public void Remove(int key)
	{
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

	public Span<int> AsSpan()
	{
		// FIXME: Dangerous if resize occurs?
		return CollectionsMarshal.AsSpan(Dense);
	}

	Enumerator GetEnumerator()
	{
		throw new NotImplementedException();
	}

	public struct Enumerator
	{
		// TODO:
		// Current
		// MoveNext
		// Reset
		// Contains
		// Intersect
	}
}
