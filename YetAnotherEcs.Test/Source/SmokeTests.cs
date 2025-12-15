using System.Numerics;
using YetAnotherEcs.General;

namespace YetAnotherEcs.Test;

[TestClass]
public class SmokeTests
{
	[Indexed]
	private record struct Tag(char Value);

	private record struct Position(Vector2 Value);

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
		Assert.AreEqual(0, entity0.Id);
		Assert.AreEqual(1, entity1.Id);
		ValidateCounts(0, 0, 0);

		entity0.Set(tag0);
		Assert.IsTrue(entity0.Has<Tag>());
		Assert.AreEqual(entity0.Get<Tag>(), tag0);
		ValidateCounts(1, 1, 0);

		entity1.Set(tag1);
		Assert.IsTrue(entity1.Has<Tag>());
		Assert.AreEqual(entity1.Get<Tag>(), tag1);
		ValidateCounts(2, 1, 1);

		entity0.Set(tag1);
		Assert.AreEqual(entity0.Get<Tag>(), tag1);
		ValidateCounts(2, 0, 2);

		entity1.Remove<Tag>();
		Assert.IsFalse(entity1.Has<Tag>());
		ValidateCounts(1, 0, 1);

		world.Recycle(entity0.Id);
		ValidateCounts(0, 0, 0);

		var entity2 = world.Create();
		Assert.AreEqual(0, entity2.Id);
		Assert.IsFalse(entity2.Has<Tag>());
		ValidateCounts(0, 0, 0);

		entity2.Set<Tag>();
		Assert.IsTrue(entity2.Has<Tag>());
		ValidateCounts(1, 0, 0);
	}

	[TestMethod]
	public void SetOperations()
	{
		var set1 = new SparseSet() { 1, 2, 3 };
		var set2 = (SparseSet)([1, 2, 3]);
		Assert.IsTrue(SetEquals(set1, set2));
		Assert.AreEqual(1, set1[0]);

		set1.Add(1);
		Assert.IsTrue(set1.Contains(1));
		Assert.AreEqual(3, set1.Count);
		Assert.IsTrue(SetEquals(set1, set2));

		set1.Add(4);
		Assert.IsTrue(set1.Contains(4));
		Assert.AreEqual(4, set1.Count);
		Assert.IsFalse(SetEquals(set1, set2));

		set1.Remove(1);
		Assert.IsFalse(set1.Contains(1));
		Assert.AreEqual(3, set1.Count);
		Assert.IsFalse(SetEquals(set1, set2));
		Assert.AreEqual(4, set1[0]);

		set2.Remove(1);
		set2.Add(4);
		Assert.IsTrue(SetEquals(set1, set2));
	}

	private static bool SetEquals(SparseSet a, SparseSet b)
	{
		if (a.Count != b.Count)
		{
			return false;
		}

		foreach (var it in a)
		{
			if (!b.Contains(it))
			{
				return false;
			}
		}

		return true;
	}
}
