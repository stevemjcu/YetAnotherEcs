namespace YetAnotherEcs.Sample;

internal class Program
{
	static void Main(string[] _)
	{
		var world = new World();
		world.Register<Position>();

		var entity = world.Create();
		entity.Set<Position>(new(6, 7));
	}

	internal record struct Position(int X, int Y) : IComponent<Position>;
}
