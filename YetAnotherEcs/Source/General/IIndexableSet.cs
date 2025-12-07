using System.Collections;

namespace YetAnotherEcs.General;

public interface IIndexableSet<T> : IEnumerable<T>
{
	int Count { get; }

	public T this[int index] { get; }

	public bool Contains(T key);

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
		private IIndexableSet<T>? Include;
		private IIndexableSet<T>? Exclude; // TODO

		private int Index = 0;

		public readonly T Current => Set[Index];

		readonly object? IEnumerator.Current => Current;

		readonly void IDisposable.Dispose() { }

		public bool MoveNext()
		{
			while (++Index < Set.Count)
			{
				if (Include?.Contains(Current) ?? true)
				{
					return true;
				}
			}

			return false;
		}

		public void Reset()
		{
			Index = 0;
		}

		readonly public Enumerator GetEnumerator()
		{
			return this;
		}

		public Enumerator Intersect(IIndexableSet<T> set)
		{
			Include = set;
			return this;
		}
	}
}
