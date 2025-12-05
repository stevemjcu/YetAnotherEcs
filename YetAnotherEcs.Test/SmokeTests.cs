namespace YetAnotherEcs.Sample;

[TestClass]
public class SmokeTests
{
	[TestMethod]
	public void Test()
	{
		var world = new World();

		var position0 = new Position(1, 2);
		var position1 = new Position(3, 4);
		var position2 = new Position(5, 6);

		var filter = world.Filter().Include<Position>().Build();
		var index = world.Index<Position>().On(position2);

		Assert.HasCount(0, filter);
		Assert.HasCount(0, index);

		var entity0 = world.Create().Set(position0);
		var entity1 = world.Create().Set(position1);

		Assert.IsTrue(entity0.Contains<Position>());
		Assert.AreEqual(entity0.Get<Position>(), position0);
		Assert.IsTrue(entity1.Contains<Position>());
		Assert.AreEqual(entity1.Get<Position>(), position1);
		Assert.HasCount(2, filter);

		entity0.Set(position2);
		Assert.AreEqual(entity0.Get<Position>(), position2);
		Assert.HasCount(2, filter);
		Assert.HasCount(1, index);

		foreach (var id in filter)
		{
			var entity = world.Get(id);
			var position = entity.Get<Position>();
			if (id == 0) Assert.AreEqual(position, position2);
			if (id == 1) Assert.AreEqual(position, position1);
		}

		entity1.Remove<Position>();
		Assert.IsFalse(entity1.Contains<Position>());
		Assert.HasCount(1, filter);

		entity0.Destroy();
		var entity2 = world.Create();
		Assert.IsFalse(entity2.Contains<Position>());
		Assert.HasCount(0, filter);
		Assert.HasCount(0, index);

		entity2.Set<Position>();
		Assert.IsTrue(entity2.Contains<Position>());
		Assert.HasCount(1, filter);
	}

	internal record struct Position(int X, int Y);

	internal record struct Player();

	internal record struct Group(int Id);
}
