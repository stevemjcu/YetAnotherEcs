namespace YetAnotherEcs.General;

public class IdPool {
	private int NextId = 0;
	private readonly Queue<int> RecycledIds = [];

	public int Assign() {
		return Assign(out _);
	}

	public int Assign(out bool recycled) {
		recycled = RecycledIds.TryDequeue(out var id);
		return recycled ? id : NextId++;
	}

	public void Recycle(int id) {
		RecycledIds.Enqueue(id);
	}
}
