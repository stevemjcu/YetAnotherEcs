namespace YetAnotherEcs;

public interface IComponent<T> where T : struct
{
	private static class TypedIdAssigner
	{
		internal static IdAssigner IdAssigner = new();
	}

	public readonly static int Id = TypedIdAssigner.IdAssigner.Assign();
}

// Example:
// internal record struct Position(int X, int Y) : IComponent<Position>;