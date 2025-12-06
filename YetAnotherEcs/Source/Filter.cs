using YetAnotherEcs.Storage;

namespace YetAnotherEcs;

public record struct Filter
{
	private int IncludeBitmask;
	private int ExcludeBitmask;

	public Filter Include<T>() where T : struct
	{
		IncludeBitmask |= Registry.GetBitmask<T>();
		return this;
	}

	public Filter Include<T, U>()
		where T : struct
		where U : struct
	{
		return Include<T>().Include<U>();
	}

	public Filter Include<T, U, V>()
		where T : struct
		where U : struct
		where V : struct
	{
		return Include<T, U>().Include<V>();
	}

	public Filter Exclude<T>() where T : struct
	{
		ExcludeBitmask |= Registry.GetBitmask<T>();
		return this;
	}

	public Filter Exclude<T, U>()
		where T : struct
		where U : struct
	{
		return Exclude<T>().Exclude<U>();
	}

	public Filter Exclude<T, U, V>()
		where T : struct
		where U : struct
		where V : struct
	{
		return Exclude<T, U>().Exclude<V>();
	}

	public readonly bool Compare(int bitmask)
	{
		return
			(bitmask & IncludeBitmask) == IncludeBitmask &&
			(bitmask & ExcludeBitmask) == 0;
	}
}
