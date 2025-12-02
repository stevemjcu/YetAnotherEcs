using YetAnotherEcs.Storage;

namespace YetAnotherEcs;

/// <summary>
/// An entity identifier associated with a component set.
/// </summary>
public record struct Entity(int Id, int Version, World World)
{
	internal readonly int Bitmask
	{
		get => World.Entities.GetBitmask(this);
		set
		{
			if (Bitmask == value) return;
			World.Entities.SetBitmask(this, value);
			World.Filters.Evaluate(Id, value);
		}
	}

	/// <summary>
	/// Destroy this entity.
	/// </summary>
	// TODO: This should remove entity from indexes too
	public readonly void Destroy() => World.Entities.Remove(this);

	/// <summary>
	/// Set a component.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	/// <param name="component">The component.</param>
	/// <returns>This entity.</returns>
	public readonly Entity Set<T>(T component = default) where T : struct
	{
		World.Components.Set(Id, component);
		Bitmask |= ComponentStore.Bitmask<T>();
		return this;
	}

	/// <summary>
	/// Remove a component.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	public readonly void Remove<T>() where T : struct
	{
		World.Components.Remove<T>(Id);
		Bitmask ^= ComponentStore.Bitmask<T>();
	}

	/// <summary>
	/// Check if a component exists.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	/// <returns>True if the component exists.</returns>
	public readonly bool Contains<T>() where T : struct =>
		(Bitmask & ComponentStore.Bitmask<T>()) > 0;

	/// <summary>
	/// Get a component.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	/// <returns>The component.</returns>
	public readonly T Get<T>() where T : struct => World.Components.Get<T>(Id);

	/// <summary>
	/// Get a component if it exists.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	/// <param name="component">The component.</param>
	/// <returns>True if the component exists.</returns>
	public readonly bool TryGet<T>(out T component) where T : struct
	{
		var has = Contains<T>();
		component = has ? Get<T>() : default;
		return has;
	}
}
