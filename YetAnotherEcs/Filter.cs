namespace YetAnotherEcs;

/// <summary>
/// Represents a filter for entities by their bitmask (component structure).
/// </summary>
public record struct Filter
{
	private int IncludeBitmask;
	private int ExcludeBitmask;

	/// <summary>
	/// Includes a type with the filter.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	/// <returns>This filter.</returns>
	public Filter Include<T>() where T : struct
	{
		IncludeBitmask |= ComponentType<T>.Bitmask;
		return this;
	}

	/// <summary>
	/// Excludes a type from the filter.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	/// <returns>This filter.</returns>
	public Filter Exclude<T>() where T : struct
	{
		ExcludeBitmask |= ComponentType<T>.Bitmask;
		return this;
	}

	/// <summary>
	/// Determines if an entity bitmask matches the filter.
	/// </summary>
	/// <param name="bitmask">The entity bitmask.</param>
	/// <returns>True if the bitmask matches; otherwise, false.</returns>
	public readonly bool Matches(int bitmask)
	{
		return
			(bitmask & IncludeBitmask) == IncludeBitmask &&
			(bitmask & ExcludeBitmask) == 0;
	}
}
