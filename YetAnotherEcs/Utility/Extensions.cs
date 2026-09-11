using System.Runtime.InteropServices;

namespace YetAnotherEcs.Utility;

public static class Extensions
{
	public static void EnsureCount<T>(this List<T> list, int index)
	{
		if (index + 1 >= list.Count)
		{
			CollectionsMarshal.SetCount(list, index + 1);
		}
	}

	public static Span<T> AsSpan<T>(this List<T> list)
	{
		return CollectionsMarshal.AsSpan<T>(list);
	}
}
