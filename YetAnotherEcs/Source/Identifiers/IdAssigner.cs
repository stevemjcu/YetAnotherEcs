namespace YetAnotherEcs;

internal class IdAssigner
{
	private int NextId;
	private readonly Stack<int> RecycledIds = [];

	public int Assign() => Assign(out _);

	public int Assign(out bool recycled)
	{
		recycled = RecycledIds.TryPop(out var id);
		return recycled ? id : NextId++;
	}

	public void Recycle(int id) => RecycledIds.Push(id);
}
