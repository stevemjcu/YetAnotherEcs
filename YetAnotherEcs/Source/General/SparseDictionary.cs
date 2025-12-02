using System.Runtime.InteropServices;

namespace YetAnotherEcs.General;

/// <summary>
/// A "dense" collection of <typeparamref name="T"/> indexed by integral keys using a "sparse" 
/// collection the size of the known key universe, as opposed to a hash table.
/// </summary>
/// <typeparam name="T">The value type.</typeparam>
public class SparseDictionary<T>
{
	private record struct Entry(int Index, T Value);

	private readonly List<int> SparseList = []; // index : subindex
	private readonly List<Entry> DenseList = []; // subindex : element

	// Operations:
	// Enumerate keys, values, or both?
	// Add operator overloads

	// O(1): Add to end of dense list
	public void Add(int index, T value)
	{
		if (index >= SparseList.Count) CollectionsMarshal.SetCount(SparseList, index + 1);
		SparseList[index] = DenseList.Count;
		DenseList.Add(new(index, value));
	}

	// O(1): Replace with end of dense list
	public void Remove(int index)
	{
		var subindex = SparseList[index];
		var last = DenseList[^1];

		(SparseList[index], SparseList[last.Index]) = (DenseList.Count - 1, subindex);
		(DenseList[subindex], DenseList[^1]) = (last with { Index = index }, default);

		DenseList.RemoveAt(DenseList.Count - 1);
	}

	// O(1) if unmanaged, O(n) if managed
	public void Clear()
	{
		SparseList.Clear();
		DenseList.Clear();
	}

	// O(1): Double lookup
	public bool Contains(int index)
	{
		if (index >= SparseList.Count) return false;
		var subindex = SparseList[index];
		return subindex < DenseList.Count && DenseList[subindex].Index == index;
	}

	// O(1): Double lookup
	public T Get(int index) => DenseList[SparseList[index]].Value;
}