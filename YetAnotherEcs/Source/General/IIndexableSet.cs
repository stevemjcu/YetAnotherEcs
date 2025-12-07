using System.Collections;

namespace YetAnotherEcs.General;

public interface IIndexableSet<T> : IEnumerable<T>
{
	int Count { get; }

	public T this[int index] { get; }

	public bool Contains(int key);

	new public Enumerator GetEnumerator()
	{
		return new(this);
	}

	IEnumerator<T> IEnumerable<T>.GetEnumerator()
	{
		return GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public struct Enumerator(IIndexableSet<T> Set) : IEnumerator<T>
	{
		private int Index = 0;

		public readonly T Current => Set[Index];

		readonly object? IEnumerator.Current => Current;

		readonly void IDisposable.Dispose() { }

		public bool MoveNext()
		{
			return ++Index < Set.Count;
		}

		public void Reset()
		{
			Index = 0;
		}
	}
}
