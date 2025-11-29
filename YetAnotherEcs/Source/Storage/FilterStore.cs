namespace YetAnotherEcs.Storage;

/// <summary>
/// Encapsulates the storage for all filters.
/// </summary>
public class FilterStore
{
	public Filter Add(World world)
	{
		return new();
	}
}
