using System.Collections;
using YetAnotherEcs.General;

namespace YetAnotherEcs.Storage;

internal class Manifest(World World) {
	private static readonly SparseSet Empty = [];

	private readonly Dictionary<Filter, SparseSet> IdSetByFilter = [];
	private readonly Dictionary<int, object> IndexStoreByType = [];

	private readonly HashSet<Filter> Filters = [];
	private readonly HashSet<int> Indexes = [];

	private Registry Registry => World.Registry;

	public void OnStructureChanged(int id, int bitmask) {
		foreach (var it in IdSetByFilter) {
			if (it.Key.Compare(bitmask)) {
				it.Value.Add(id);
			}
			else {
				it.Value.Remove(id);
			}
		}
	}

	public void OnIndexAdded<T>(int id, T index) where T : struct {
		var store = GetIndexStore<T>();

		if (!store.TryGetValue(index, out var set)) {
			store[index] = set = [];
		}

		set.Add(id);
	}

	public void OnIndexRemoved<T>(int id, T index) where T : struct {
		var store = GetIndexStore<T>();

		if (store.TryGetValue(index, out var set)) {
			set.Remove(id);
		}
	}

	public void OnEntityRecycled(int id) {
		foreach (var it in IdSetByFilter.Values) {
			it.Remove(id);
		}

		foreach (IDictionary store in IndexStoreByType.Values) {
			foreach (SparseSet it in store.Values) {
				it.Remove(id);
			}
		}
	}

	public SparseSet View(Filter filter) {
		if (Filters.Add(filter)) {
			Build(filter);
		}

		return IdSetByFilter[filter];
	}

	public SparseSet View<T>(T index) where T : struct {
		if (Indexes.Add(Component<T>.Id)) {
			Build<T>(); // Is this needed anymore?
		}

		return GetIndexStore<T>().TryGetValue(index, out var set) ? set : Empty;
	}

	private Dictionary<T, SparseSet> GetIndexStore<T>() where T : struct {
		var typeId = Component<T>.Id;

		if (!IndexStoreByType.TryGetValue(typeId, out var value)) {
			value = new Dictionary<T, SparseSet>();
			IndexStoreByType.Add(typeId, value);
		}

		// Maps component to entity set
		return (Dictionary<T, SparseSet>)value;
	}

	private void Build(Filter filter) {
		IdSetByFilter[filter] = [];

		foreach (var (id, bitmask) in Registry.GetEntities()) {
			OnStructureChanged(id, bitmask);
		}
	}

	private void Build<T>() where T : struct {
		if (!Component<T>.Indexed) {
			throw new InvalidOperationException(
				$"Cannot build an index for the non-indexed component {typeof(T)}.");
		}

		foreach (var (id, _) in Registry.GetEntities()) {
			if (Registry.TryGet<T>(id, out var value)) {
				OnIndexAdded<T>(id, value);
			}
		}
	}
}
