using System.Numerics;
using YetAnotherEcs.General;

namespace YetAnotherEcs.Test;

[TestClass]
public class SmokeTests
{
	[TestMethod]
	public void WorldOperations()
	{
		var world = new World();

		var filter = new Filter().Include<Tag>();
		var tag0 = new Tag('0');
		var tag1 = new Tag('1');

		void ValidateCounts(int a, int b, int c)
		{
			Assert.AreEqual(a, world.View(filter).Count);
			Assert.AreEqual(b, world.View(tag0).Count);
			Assert.AreEqual(c, world.View(tag1).Count);
		}

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

		world.Set<Tag>(entity2, new());
		Assert.IsTrue(world.Has<Tag>(entity2));
		ValidateCounts(1, 0, 0);
	}

	[TestMethod]
	public void SetOperations()
	{
		var set1 = new SparseSet() { 1, 2, 3 };
		var set2 = (SparseSet)([1, 2, 3]);
		Assert.IsTrue(SetEquals(set1, set2));

		set1.Add(1);
		Assert.IsTrue(set1.Contains(1));
		Assert.HasCount(3, set1);
		Assert.IsTrue(SetEquals(set1, set2));

		set1.Add(4);
		Assert.IsTrue(set1.Contains(4));
		Assert.HasCount(4, set1);
		Assert.IsFalse(SetEquals(set1, set2));

		set1.Remove(1);
		Assert.IsFalse(set1.Contains(1));
		Assert.HasCount(3, set1);
		Assert.IsFalse(SetEquals(set1, set2));

		var count = 0;
		foreach (var it in ((IIndexableSet<int>)set1).Intersect(set2))
		{
			Assert.IsTrue(set1.Contains(it));
			Assert.IsTrue(set2.Contains(it));
			count++;
		}
		Assert.AreEqual(2, count);

		set2.Remove(1);
		set2.Add(4);
		Assert.IsTrue(SetEquals(set1, set2));
	}

	[Indexed]
	internal record struct Tag(char Value);

	internal record struct Position(Vector2 Value);

	internal bool SetEquals<T>(IIndexableSet<T> set1, IIndexableSet<T> set2)
	{
		if (set1.Count != set2.Count)
		{
			return false;
		}

		foreach (var it in set1)
		{
			if (!set2.Contains(it))
			{
				return false;
			}
		}

		return true;
	}
}
