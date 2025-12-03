using YetAnotherEcs.Storage;

namespace YetAnotherEcs;

/// <summary>
/// An entity identifier associated with a component set.
/// </summary>
public record struct Entity(int Id, int Version, World World)
{
	internal readonly int Bitmask
	{
		get => World.Entities.GetBitmask(Id);
		set
		{
			if (Bitmask == value) return;
			World.Entities.SetBitmask(Id, value);
			World.Filters.Evaluate(Id, value);
		}
	}

	/// <summary>
	/// Destroy this entity.
	/// </summary>
	public readonly void Destroy()
	{
		World.Components.ClearIndices(Id, Bitmask);
		Bitmask = 0;
		World.Entities.Remove(Id);
	}

	/// <summary>
	/// Set a component.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	/// <param name="component">The component.</param>
	/// <returns>This entity.</returns>
	public readonly Entity Set<T>(T component = default) where T : struct
	{
		World.Components.Store<T>().Set(Id, component);
		Bitmask |= ComponentStore.Bitmask<T>();
		return this;
	}

	/// <summary>
	/// Remove a component.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	public readonly void Remove<T>() where T : struct
	{
		World.Components.Store<T>().Remove(Id);
		Bitmask ^= ComponentStore.Bitmask<T>();
	}

	/// <summary>
	/// Check if a component exists.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	/// <returns>True if the component exists.</returns>
	public readonly bool Contains<T>() where T : struct => (Bitmask & ComponentStore.Bitmask<T>()) > 0;

	/// <summary>
	/// Get a component.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	/// <returns>The component.</returns>
	public readonly T Get<T>() where T : struct => World.Components.Store<T>().Get(Id);

	/// <summary>
	/// Get a component if it exists.
	/// </summary>
	/// <typeparam name="T">The component type.</typeparam>
	/// <param name="component">The component.</param>
	/// <returns>True if the component exists.</returns>
	public readonly bool TryGetValue<T>(out T component) where T : struct
	{
		var has = Contains<T>();
		component = has ? Get<T>() : default;
		return has;
	}
}
