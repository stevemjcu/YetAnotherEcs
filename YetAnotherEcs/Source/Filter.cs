namespace YetAnotherEcs;

public record struct Filter
{
	private int IncludeBitmask;
	private int ExcludeBitmask;

	public Filter Include<T>() where T : struct, IComponent
	{
		IncludeBitmask |= IComponent.GetBitmask<T>();
		return this;
	}

	public Filter Include<T, U>()
		where T : struct, IComponent
		where U : struct, IComponent
	{
		return Include<T>().Include<U>();
	}

	public Filter Include<T, U, V>()
		where T : struct, IComponent
		where U : struct, IComponent
		where V : struct, IComponent
	{
		return Include<T, U>().Include<V>();
	}

	public Filter Include<T, U, V, W>()
		where T : struct, IComponent
		where U : struct, IComponent
		where V : struct, IComponent
		where W : struct, IComponent
	{
		return Include<T, U, V>().Include<W>();
	}

	public Filter Include<T, U, V, W, X>()
		where T : struct, IComponent
		where U : struct, IComponent
		where V : struct, IComponent
		where W : struct, IComponent
		where X : struct, IComponent
	{
		return Include<T, U, V, W>().Include<X>();
	}

	public Filter Exclude<T>() where T : struct, IComponent
	{
		ExcludeBitmask |= IComponent.GetBitmask<T>();
		return this;
	}

	public Filter Exclude<T, U>()
		where T : struct, IComponent
		where U : struct, IComponent
	{
		return Exclude<T>().Exclude<U>();
	}

	public Filter Exclude<T, U, V>()
		where T : struct, IComponent
		where U : struct, IComponent
		where V : struct, IComponent
	{
		return Exclude<T, U>().Exclude<V>();
	}

	public Filter Exclude<T, U, V, W>()
		where T : struct, IComponent
		where U : struct, IComponent
		where V : struct, IComponent
		where W : struct, IComponent
	{
		return Exclude<T, U, V>().Exclude<W>();
	}

	public Filter Exclude<T, U, V, W, X>()
		where T : struct, IComponent
		where U : struct, IComponent
		where V : struct, IComponent
		where W : struct, IComponent
		where X : struct, IComponent
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
