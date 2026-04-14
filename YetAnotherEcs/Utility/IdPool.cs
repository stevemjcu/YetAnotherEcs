namespace YetAnotherEcs.Utility;

/// <summary>
/// Manages a pool of IDs.
/// </summary>
public class IdPool
{
	private int NextId = 0;
	private readonly Queue<int> RecycledIds = [];

	/// <summary>
	/// Reserves an ID from the pool.
	/// </summary>
	/// <returns>The assigned ID.</returns>
	public int Assign()
	{
		return RecycledIds.TryDequeue(out var id) ? id : NextId++;
	}

	/// <summary>
	/// Returns an ID to the pool.
	/// </summary>
	/// <param name="id">The recycled ID.</param>
	public void Recycle(int id)
	{
		RecycledIds.Enqueue(id);
	}
}
