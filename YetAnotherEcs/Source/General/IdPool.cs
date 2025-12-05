namespace YetAnotherEcs.General;

public class IdPool
{
	private int NextId = 0;
	private readonly Stack<int> RecycledIds = [];

	public int Assign()
	{
		return Assign(out _);
	}

	public int Assign(out bool recycled)
	{
		recycled = RecycledIds.TryPop(out var id);
		return recycled ? id : NextId++;
	}

	public void Recycle(int id)
	{
		RecycledIds.Push(id);
	}
}
