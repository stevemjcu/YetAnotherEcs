namespace YetAnotherEcs;

public record struct Filter
{
	private int IncludeBitmask;
	private int ExcludeBitmask;

	public Filter Include<T>() where T : struct
	{
		IncludeBitmask |= Component<T>.Bitmask;
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

	public Filter Include<T, U, V, W>()
		where T : struct
		where U : struct
		where V : struct
		where W : struct
	{
		return Include<T, U, V>().Include<W>();
	}

	public Filter Include<T, U, V, W, X>()
		where T : struct
		where U : struct
		where V : struct
		where W : struct
		where X : struct
	{
		return Include<T, U, V, W>().Include<X>();
	}

	public Filter Exclude<T>() where T : struct
	{
		ExcludeBitmask |= Component<T>.Bitmask;
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

	public Filter Exclude<T, U, V, W>()
		where T : struct
		where U : struct
		where V : struct
		where W : struct
	{
		return Exclude<T, U, V>().Exclude<W>();
	}

	public Filter Exclude<T, U, V, W, X>()
		where T : struct
		where U : struct
		where V : struct
		where W : struct
		where X : struct
	{
		return Exclude<T, U, V, W>().Exclude<X>();
	}

	public readonly bool Compare(int bitmask)
	{
		return
			(bitmask & IncludeBitmask) == IncludeBitmask &&
			(bitmask & ExcludeBitmask) == 0;
	}
}
