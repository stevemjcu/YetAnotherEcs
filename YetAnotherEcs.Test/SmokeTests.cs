using System.Numerics;
using YetAnotherEcs.Utility;

namespace YetAnotherEcs.Test;

[TestClass]
public class SmokeTests
{
	[IndexedComponent]
	private record struct Tag(char Value);

	[Component]
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
			Assert.AreEqual(a, world.GetView(filter).Count);
			Assert.AreEqual(b, world.GetView(tag0).Count);
			Assert.AreEqual(c, world.GetView(tag1).Count);
		}

		var entity0 = world.CreateEntity();
		var entity1 = world.CreateEntity();
		Assert.AreEqual(0, entity0);
		Assert.AreEqual(1, entity1);
		ValidateCounts(0, 0, 0);

		world.SetComponent(entity0, tag0);
		Assert.IsTrue(world.HasComponent<Tag>(entity0));
		Assert.AreEqual(world.GetComponent<Tag>(entity0), tag0);
		ValidateCounts(1, 1, 0);

		world.SetComponent(entity1, tag1);
		Assert.IsTrue(world.HasComponent<Tag>(entity1));
		Assert.AreEqual(world.GetComponent<Tag>(entity1), tag1);
		ValidateCounts(2, 1, 1);

		world.SetComponent(entity0, tag1);
		Assert.AreEqual(world.GetComponent<Tag>(entity0), tag1);
		ValidateCounts(2, 0, 2);

		world.RemoveComponent<Tag>(entity1);
		Assert.IsFalse(world.HasComponent<Tag>(entity1));
		ValidateCounts(1, 0, 1);

		world.DeleteEntity(entity0);
		ValidateCounts(0, 0, 0);

		var entity2 = world.CreateEntity();
		Assert.AreEqual(0, entity2);
		Assert.IsFalse(world.HasComponent<Tag>(entity2));
		ValidateCounts(0, 0, 0);

		world.SetComponent<Tag>(entity2);
		Assert.IsTrue(world.HasComponent<Tag>(entity2));
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
