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
			Assert.HasCount(a, world.Query(filter));
			Assert.HasCount(b, world.Query(tag0));
			Assert.HasCount(c, world.Query(tag1));
		}

		world.IndexOn(filter);
		world.IndexOn<Tag>();

		var entity0 = world.Create();
		var entity1 = world.Create();
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
			.Query(filter)
			.Select(world.Get<Tag>)
			.All(it => it == tag1));

		world.Remove<Tag>(entity1);
		Assert.IsFalse(world.Has<Tag>(entity1));
		ValidateCounts(1, 0, 1);

		world.Destroy(entity0);
		ValidateCounts(0, 0, 0);

		var entity2 = world.Create();
		Assert.IsFalse(world.Has<Tag>(entity2));
		ValidateCounts(0, 0, 0);

		world.Set<Tag>(entity2);
		Assert.IsTrue(world.Has<Tag>(entity2));
		ValidateCounts(1, 0, 0);
	}

	internal record struct Tag(char Value);

	internal record struct Unique();
}
