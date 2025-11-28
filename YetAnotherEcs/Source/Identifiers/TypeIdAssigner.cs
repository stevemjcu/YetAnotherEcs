namespace YetAnotherEcs;

internal static class TypeIdAssigner
{
	internal static IdAssigner IdAssigner = new();
}

internal static class TypeIdAssigner1<T>
{
	public static int Id;

	static TypeIdAssigner1()
	{
		Id = TypeIdAssigner.IdAssigner.Assign();
	}
}
