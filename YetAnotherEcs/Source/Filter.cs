namespace YetAnotherEcs;

/// <summary>
/// An identifier associated with an entity set.
/// </summary>
internal record struct Filter
{
	public int Signature;

	public Filter Include<T>() => this;

	public Filter Exclude<T>() => this;

	public Filter Build() => this;
}
