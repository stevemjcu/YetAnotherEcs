using YetAnotherEcs.Storage;

namespace YetAnotherEcs;

public record struct Filter
{
	private int IncludeBitmask;
	private int ExcludeBitmask;

	public Filter Include<T>() where T : struct
	{
		IncludeBitmask |= Registry.TypeBitmask<T>();
		return this;
	}

	public Filter Exclude<T>() where T : struct
	{
		ExcludeBitmask |= Registry.TypeBitmask<T>();
		return this;
	}

	public readonly bool Compare(int bitmask)
	{
		return
			(bitmask & IncludeBitmask) == IncludeBitmask &&
			(bitmask & ExcludeBitmask) == 0;
	}
}
