namespace YetAnotherEcs;

/// <summary>
/// A filter signature associated with an entity set.
/// </summary>
public record struct Filter(int Signature, World World)
{
	/// <summary>
	/// Include a component type on the filter.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	/// <returns>This filter.</returns>
	public Filter Include<T>() where T : struct => this;

	/// <summary>
	/// Include a component index on the filter.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	/// <param name="component">The component.</param>
	/// <returns>This filter.</returns>
	public Filter Include<T>(T component) where T : struct => this;

	/// <summary>
	/// Exclude a component type on the filter.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	/// <returns>This filter.</returns>
	public Filter Exclude<T>() where T : struct => this;

	/// <summary>
	/// Exclude a component index on the filter.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	/// <param name="component">The component.</param>
	/// <returns>This filter.</returns>
	public Filter Exclude<T>(T component) where T : struct => this;

	/// <summary>
	/// Create a signature and register for automatic updates.
	/// </summary>
	/// <returns>This filter.</returns>
	public Filter Build() => this;

	/// <summary>
	/// Read the entity set from storage.
	/// </summary>
	/// <returns>The entity set.</returns>
	public IReadOnlySet<Entity> Read() => throw new NotImplementedException();
}
