namespace YetAnotherEcs;

/// <summary>
/// An identifier associated with an entity set.
/// </summary>
public record struct Filter
{
	public int Signature;

	public Filter Include<T>() where T : struct, IComponent<T> => this;

	public Filter Exclude<T>() where T : struct, IComponent<T> => this;

	public Filter Build() => this;
}
