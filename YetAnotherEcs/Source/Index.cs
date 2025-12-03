using System.Collections;

namespace YetAnotherEcs;

/// <summary>
/// A component index associated with an entity set.
/// </summary>
/// <typeparam name="T">The component type.</typeparam>
public struct Index<T>(World World) : IEnumerable<int> where T : struct
{
	private T Target;

	public Index<T> On(T index)
	{
		Target = index;
		World.Components.Store<T>().Initialize(index);
		return this;
	}

	/// <summary>
	/// Returns true if the ID exists in this entity set.
	/// </summary>
	/// <param name="id">The entity ID.</param>
	/// <returns>True if this set contains the entity ID.</returns>
	public readonly bool Contains(int id) =>
		World.Components.Store<T>().Contains(Target, id);

	/// <summary>
	/// Enumerates this entity set by ID.
	/// </summary>
	public readonly HashSet<int>.Enumerator GetEnumerator() =>
		World.Components.Store<T>().GetEnumerator(Target);

	readonly IEnumerator<int> IEnumerable<int>.GetEnumerator() => GetEnumerator();

	readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
