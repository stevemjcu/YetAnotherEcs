namespace YetAnotherEcs;

public readonly record struct Filter(int IncludeBitmask, int ExcludeBitmask)
{
	public bool Compare(int bitmask)
	{
		return
			(bitmask & IncludeBitmask) == IncludeBitmask &&
			(bitmask & ExcludeBitmask) == 0;
	}
}
