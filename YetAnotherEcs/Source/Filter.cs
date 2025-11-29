namespace YetAnotherEcs;

/// <summary>
/// An identifier associated with an entity set.
/// </summary>
public record struct Filter(int Signature)
{
	public Filter Include<T>() where T : struct, IComponent<T> => this;

	public Filter Include<T>(T value) where T : struct, IComponent<T> => this;

	public Filter Exclude<T>() where T : struct, IComponent<T> => this;

	public Filter Exclude<T>(T value) where T : struct, IComponent<T> => this;

	public Filter Build() => this;

	public IReadOnlySet<Entity> Read() => throw new NotImplementedException();
}
