namespace YetAnotherEcs.Sample;

internal class Program
{
	static void Main(string[] _)
	{
		var world = new World();

		var position0 = new Position(1, 2);
		var position1 = new Position(3, 4);
		var position2 = new Position(5, 6);

		var entity0 = world.Create().Set(position0);
		var entity1 = world.Create().Set(position1);

		var filter1 = world.Filter().Include<Position>().Register();

		Assert(entity0.Contains<Position>());
		Assert(entity1.Contains<Position>());

		Assert(entity0.Get<Position>() == position0);
		Assert(entity1.Get<Position>() == position1);

		entity0.Set(position2);
		Assert(entity0.Get<Position>() == position2);
		Assert(world.Query(filter1).Count == 1);

		entity1.Remove<Position>();
		Assert(!entity1.Contains<Position>());
		Assert(world.Query(filter1).Count == 1);

		// Recycle entity
		world.Destroy(entity0);
		var entity2 = world.Create();
		Assert(!entity2.Contains<Position>());

		entity2.Set<Position>();
		Assert(entity2.Contains<Position>());
		Assert(world.Query(filter1).Count == 1);

		entity1.Set<Player>();
		Assert(world.Get<Player>() == entity1);
	}

	private static void Assert(bool condition)
	{
		if (!condition) throw new Exception();
	}

	internal record struct Position(int X, int Y);

	internal record struct Player();
}
