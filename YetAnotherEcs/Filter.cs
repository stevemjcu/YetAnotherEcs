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

	public Filter Exclude<T>() where T : struct
	{
		ExcludeBitmask |= Component<T>.Bitmask;
		return this;
	}

	public readonly bool Compare(int bitmask)
	{
		return
			(bitmask & IncludeBitmask) == IncludeBitmask &&
			(bitmask & ExcludeBitmask) == 0;
	}
}
