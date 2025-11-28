namespace YetAnotherEcs.General;

internal class IdAssigner
{
	private int NextId;
	private Stack<int> RecycledIds = [];

	public int Assign() => RecycledIds.TryPop(out var id) ? id : NextId++;

	public int Assign(out bool recycled)
	{
		recycled = RecycledIds.TryPop(out var id);
		return recycled ? id : NextId++;
	}

	public void Recycle(int id) => RecycledIds.Push(id);
}
