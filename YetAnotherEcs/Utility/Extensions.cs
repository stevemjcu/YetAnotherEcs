using System.Runtime.InteropServices;

namespace YetAnotherEcs.Utility;

public static class Extensions
{
	public static void EnsureCount<T>(this List<T> list, int count)
	{
		if (count > list.Count)
		{
			CollectionsMarshal.SetCount(list, count);
		}
	}

	public static Span<T> AsSpan<T>(this List<T> list)
	{
		return CollectionsMarshal.AsSpan<T>(list);
	}
}
