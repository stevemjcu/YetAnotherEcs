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

	public Enumerator Intersect(IIndexableSet<T> set)
	{
		return GetEnumerator().Intersect(set);
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
		private IIndexableSet<T>? InnerSet0;
		private IIndexableSet<T>? InnerSet1;

		private int Index = -1;

		public readonly T Current => Set[Index];

		readonly object? IEnumerator.Current => Current;

		readonly void IDisposable.Dispose() { }

		public bool MoveNext()
		{
			while (++Index < Set.Count)
			{
				if (
					(InnerSet0?.Contains(Current) ?? true) &&
					(InnerSet1?.Contains(Current) ?? true))
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

		internal Enumerator Intersect(IIndexableSet<T> set)
		{
			InnerSet0 = set;
			return this;
		}

		internal Enumerator Intersect(IIndexableSet<T> set1, IIndexableSet<T> set2)
		{
			(InnerSet0, InnerSet1) = (set1, set2);
			return this;
		}
	}
}
