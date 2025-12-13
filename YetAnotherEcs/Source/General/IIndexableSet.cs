using System.Collections;

namespace YetAnotherEcs.General;

public interface IIndexableSet<T> : IEnumerable<T>
{
	int Count { get; }

	public T this[int index] { get; }

	public bool Contains(T item);

	new public Enumerator GetEnumerator()
	{
		return new(this);
	}

	public Enumerator Intersect(IIndexableSet<T> set, bool enumerate = false)
	{
		return GetEnumerator().Intersect(set, enumerate);
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
		private IIndexableSet<T>? InnerSet;

		private int Index = -1;

		public readonly T Current => Set[Index];

		readonly object? IEnumerator.Current => Current;

		readonly void IDisposable.Dispose() { }

		public bool MoveNext()
		{
			while (++Index < Set.Count)
			{
				if (InnerSet?.Contains(Current) ?? true)
				{
					return true;
				}
			}

			return false;
		}

		public void Reset()
		{
			Index = -1;
		}

		readonly public Enumerator GetEnumerator()
		{
			return this;
		}

		internal Enumerator Intersect(IIndexableSet<T> set, bool enumerate)
		{
			InnerSet = set;

			if (enumerate)
			{
				(Set, InnerSet) = (InnerSet, Set);
			}

			return this;
		}
	}
}
