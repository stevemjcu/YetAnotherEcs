using YetAnotherEcs.Storage;

namespace YetAnotherEcs;

public struct FilterBuilder(World World)
{
	private int IncludeBitmask;
	private int ExcludeBitmask;

	public FilterBuilder Include<T>() where T : struct
	{
		IncludeBitmask |= EntityStore.TypeBitmask<T>();
		return this;
	}

	public FilterBuilder Exclude<T>() where T : struct
	{
		ExcludeBitmask |= EntityStore.TypeBitmask<T>();
		return this;
	}

	public readonly Filter Build()
	{
		var filter = new Filter(IncludeBitmask, ExcludeBitmask);
		// TODO: Register filter
		return filter;
	}

}
