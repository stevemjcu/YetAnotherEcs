using YetAnotherEcs.General;

namespace YetAnotherEcs.Storage;

internal class Manifest
{
	private readonly Dictionary<Filter, SparseSet> SetByFilter = [];
	private readonly Dictionary<int, SparseSet> SetByHash = [];
	private readonly SparseSet Empty = [];

	public void OnStructureChanged(int id, int bitmask)
	{
		foreach (var it in SetByFilter)
		{
			if (it.Key.Compare(bitmask))
			{
				it.Value.Add(id);
			}
			else
			{
				it.Value.Remove(id);
			}
		}
	}

	public void OnIndexAdded(int id, int hash)
	{
		if (!SetByHash.TryGetValue(hash, out var set))
		{
			SetByHash[hash] = set = [];
		}

		set.Add(id);
	}

	public void OnIndexRemoved(int id, int hash)
	{
		if (SetByHash.TryGetValue(hash, out var set))
		{
			set.Remove(id);

			if (set.Count == 0)
			{
				SetByHash.Remove(hash);
			}
		}
	}

	public void OnEntityRecycled(int id)
	{
		foreach (var it in SetByFilter.Values)
		{
			it.Remove(id);
		}

		foreach (var it in SetByHash.Values)
		{
			it.Remove(id);
		}
	}

	public IIndexableSet<int> View(Filter filter)
	{
		return SetByFilter[filter];
	}

	public IIndexableSet<int> View<T>(T index) where T : struct, IComponent
	{
		var hash = IComponent.GetHashCode(index);
		return SetByHash.TryGetValue(hash, out var set) ? set : Empty;
	}
}
