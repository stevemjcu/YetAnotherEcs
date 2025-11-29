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

		Assert(entity0.Has<Position>());
		Assert(entity1.Has<Position>());

		Assert(entity0.Get<Position>() == position0);
		Assert(entity1.Get<Position>() == position1);

		entity0.Set(position2);
		Assert(entity0.Get<Position>() == position2);

		entity1.Remove<Position>();
		Assert(!entity1.Has<Position>());

		// Recycle entity 
		entity0.Destroy();
		var entity2 = world.Create();

		Assert(!entity2.Has<Position>());
		entity2.Set<Position>();
		Assert(entity2.Has<Position>());

		entity1.Set<Player>();
		Assert(world.Singleton<Player>() == entity1);
	}

	private static void Assert(bool condition)
	{
		if (condition) return;
		throw new Exception();
	}

	internal record struct Position(int X, int Y);

	internal record struct Player();
}
