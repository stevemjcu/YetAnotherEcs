namespace YetAnotherEcs;

/// <summary>
/// Represents a component set associated with an entity ID.
/// </summary>
public record struct Entity(int Id, int Version, World World)
{
	internal readonly int Bitmask
	{
		get => World.Entities.GetBitmask(this);
		set => World.Entities.SetBitmask(this, value);
	}

	/// <summary>
	/// Copy this entity's components.
	/// </summary>
	/// <returns>The new entity.</returns>
	public readonly Entity Copy() => World.Entities.Copy(this);

	/// <summary>
	/// Destroy this entity.
	/// </summary>
	public readonly void Destroy() => World.Entities.Destroy(this);

	/// <summary>
	/// Set a component.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	/// <param name="component">The component.</param>
	/// <returns>This entity.</returns>
	public readonly Entity Set<T>(T component = default) where T : struct, IComponent<T>
	{
		World.Components.Set(Id, component);
		Bitmask |= 1 << World.Components.Id<T>();
		return this;
	}

	/// <summary>
	/// Remove a component.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	public readonly void Remove<T>() where T : struct, IComponent<T>
	{
		World.Components.Remove<T>(Id);
		Bitmask ^= 1 << World.Components.Id<T>();
	}

	/// <summary>
	/// Check if a component exists.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	/// <returns>True if the component exists.</returns>
	public readonly bool Has<T>() where T : struct, IComponent<T> =>
		(Bitmask & (1 << World.Components.Id<T>())) > 0;

	/// <summary>
	/// Get a component.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	/// <returns>The component.</returns>
	public readonly T Get<T>() where T : struct, IComponent<T> =>
		World.Components.Get<T>(Id);

	/// <summary>
	/// Get a component if it exists.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	/// <param name="component">The component.</param>
	/// <returns>True if the component exists.</returns>
	public readonly bool TryGet<T>(out T component) where T : struct, IComponent<T>
	{
		var has = Has<T>();
		component = has ? Get<T>() : default;
		return has;
	}
}
