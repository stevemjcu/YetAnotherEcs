namespace YetAnotherEcs;

/// <summary>
/// An indexed component type associated with many entity sets.
/// </summary>
/// <typeparam name="T">The component type.</typeparam>
public struct Index<T>(World World) where T : struct
{
	/// <summary>
	/// Get the associated entity ID set for the specified index.
	/// </summary>
	/// <param name="index">The component.</param>
	/// <returns>The entity ID set.</returns>
	// TODO: Can expose enumerator and contains method rather than reveal implementation
	public readonly IReadOnlySet<int> AsSet(T index) => World.Components.AsSet(index);
}
