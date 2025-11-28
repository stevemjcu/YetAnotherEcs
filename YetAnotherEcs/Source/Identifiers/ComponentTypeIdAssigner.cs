namespace YetAnotherEcs;

internal static class ComponentTypeIdAssigner
{
	internal static IdAssigner IdAssigner = new();
}

internal static class ComponentTypeIdAssigner<T>
{
	public static int Id;

	static ComponentTypeIdAssigner()
	{
		Id = ComponentTypeIdAssigner.IdAssigner.Assign();
	}
}
