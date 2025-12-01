namespace YetAnotherEcs.General;

public class IdPool(int start = 0)
{
	private int NextId = start;
	private readonly Stack<int> RecycledIds = [];

	public int Assign() => Assign(out _);

	public int Assign(out bool recycled)
	{
		recycled = RecycledIds.TryPop(out var id);
		return recycled ? id : NextId++;
	}

	public void Recycle(int id) => RecycledIds.Push(id);
}
