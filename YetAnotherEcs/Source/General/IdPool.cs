namespace YetAnotherEcs.General;

/// <summary>
/// An ID pool which assigns a 0-indexed, incrementing ID which can be recycled.
/// </summary>
public class IdPool
{
	private int NextId = 0;
	private readonly Stack<int> RecycledIds = [];

	public int Assign() => Assign(out _);

	public int Assign(out bool recycled)
	{
		recycled = RecycledIds.TryPop(out var id);
		return recycled ? id : NextId++;
	}

	public void Recycle(int id) => RecycledIds.Push(id);
}
