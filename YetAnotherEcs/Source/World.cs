namespace YetAnotherEcs;

/// <summary>
/// The storage for all entities and components.
/// </summary>
public class World
{
	private static readonly List<World> Worlds = [];

	public Entity Create() => default;

	public Entity Clone(Entity entity) => default;

	public void Destroy(Entity entity) { }

	public void Index<T>() { }

	public void Filter() { }

	Entity Get<T>() => default;

	Entity Get<T>(T index) => default;

	Entity Get(Filter filter) => default;
}
