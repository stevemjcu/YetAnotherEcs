using System.Collections;
using YetAnotherEcs.Utility;

namespace YetAnotherEcs;

/// <summary>
/// An arbitrary set of entities.
/// </summary>
public readonly struct View : IEnumerable<Entity>
{
	private readonly World World;
	private readonly SparseSet Set;

	internal View(World world, SparseSet set)
	{
		World = world;
		Set = set;
	}

	public readonly int Count => Set.Count;

	public readonly Entity this[int index] => new(World, Set[index]);

	public readonly bool Contains(Entity entity)
	{
		return Set.Contains(entity.Id);
	}

	public readonly ViewEnumerator GetEnumerator()
	{
		return new(World, Set);
	}

	IEnumerator<Entity> IEnumerable<Entity>.GetEnumerator()
	{
		return GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	/// <summary>
	/// Traverses the set in reverse to avoid invalidation.
	/// </summary>
	public struct ViewEnumerator : IEnumerator<Entity>
	{
		private readonly World World;
		private readonly SparseSet Set;

		private int Index;

		public readonly Entity Current => new(World, Set[Index]);

		object IEnumerator.Current => Current;

		internal ViewEnumerator(World world, SparseSet set)
		{
			World = world;
			Set = set;
			Index = set.Count;
		}

		public bool MoveNext()
		{
			while (--Index >= 0)
			{
				return true;
			}

			return false;
		}

		public void Reset()
		{
			Index = Set.Count;
		}

		void IDisposable.Dispose()
		{
		}
	}
}
