namespace YetAnotherEcs.Test.Source;

[TestClass]
public class Smoke
{
	[TestMethod]
	public void Test()
	{
		var world = new World();

		var filter = new Filter().Include<Tag>();
		var tag0 = new Tag('0');
		var tag1 = new Tag('1');

		void ValidateCounts(int a, int b, int c)
		{
			Assert.AreEqual(a, world.View(filter).Length);
			Assert.AreEqual(b, world.View(tag0).Length);
			Assert.AreEqual(c, world.View(tag1).Length);
		}

		world.View(filter);
		world.View<Tag>(default);

		var entity0 = world.Create();
		var entity1 = world.Create();
		Assert.AreEqual(0, entity0);
		Assert.AreEqual(1, entity1);
		ValidateCounts(0, 0, 0);

		world.Set(entity0, tag0);
		Assert.IsTrue(world.Has<Tag>(entity0));
		Assert.AreEqual(world.Get<Tag>(entity0), tag0);
		ValidateCounts(1, 1, 0);

		world.Set(entity1, tag1);
		Assert.IsTrue(world.Has<Tag>(entity1));
		Assert.AreEqual(world.Get<Tag>(entity1), tag1);
		ValidateCounts(2, 1, 1);

		world.Set(entity0, tag1);
		Assert.AreEqual(world.Get<Tag>(entity0), tag1);
		ValidateCounts(2, 0, 2);

		Assert.IsTrue(world
			.View(filter)
			.ToArray()
			.Select(world.Get<Tag>)
			.All(it => it == tag1));

		world.Remove<Tag>(entity1);
		Assert.IsFalse(world.Has<Tag>(entity1));
		ValidateCounts(1, 0, 1);

		world.Recycle(entity0);
		ValidateCounts(0, 0, 0);

		var entity2 = world.Create();
		Assert.AreEqual(0, entity2);
		Assert.IsFalse(world.Has<Tag>(entity2));
		ValidateCounts(0, 0, 0);

		world.Set<Tag>(entity2);
		Assert.IsTrue(world.Has<Tag>(entity2));
		ValidateCounts(1, 0, 0);
	}

	internal record struct Tag(char Value);

	internal record struct Unique();
}
