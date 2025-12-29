using YetAnotherEcs.Storage;

namespace YetAnotherEcs;

public class World {
	internal Registry Registry;
	internal Manifest Manifest;

	public World() {
		Registry = new(this);
		Manifest = new(this);
	}

	public Entity Create() {
		return new(this, Registry.Create());
	}

	public Entity Get(int id) {
		return new(this, id);
	}

	public void Recycle(int id) {
		Registry.Recycle(id);
	}

	#region Component API

	internal void Set<T>(int id, T value = default) where T : struct {
		Registry.Set(id, value);
	}

	internal void Remove<T>(int id) where T : struct {
		Registry.Remove<T>(id);
	}

	internal bool Has<T>(int id) where T : struct {
		return Registry.Has<T>(id);
	}

	internal T Get<T>(int id) where T : struct {
		return Registry.Get<T>(id);
	}

	#endregion

	public View View(Filter filter) {
		return new(this, Manifest.View(filter));
	}

	public View View<T>(T index) where T : struct {
		return new(this, Manifest.View(index));
	}
}
