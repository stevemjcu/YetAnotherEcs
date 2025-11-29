using YetAnotherEcs.Storage;

namespace YetAnotherEcs;

/// <summary>
/// Manages a set of entities and their storage. 
/// </summary>
public class World
{
	internal EntityStore Entities = new();
	internal ComponentStore Components = new();

	/// <summary>
	/// Create an entity.
	/// </summary>
	/// <returns>The entity.</returns>
	public Entity Create() => Entities.Add(this);

	//public Filter Filter() =>
	//	throw new NotImplementedException();

	//public Entity Single<T>() where T : struct, IComponent<T> =>
	//	throw new NotImplementedException();

	//public Entity Single<T>(T index) where T : struct, IComponent<T> =>
	//	throw new NotImplementedException();

	//public IReadOnlySet<Entity> Get<T>(T index) where T : struct, IComponent<T> =>
	//	throw new NotImplementedException();

	//public IReadOnlySet<Entity> Get(Filter filter) =>
	//	throw new NotImplementedException();
}
