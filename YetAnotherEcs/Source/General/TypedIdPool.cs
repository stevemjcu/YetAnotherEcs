namespace YetAnotherEcs.General;

public static class TypedIdPool<Context, T> {
	public readonly static int Id;

	static TypedIdPool() {
		Id = TypedIdPool<Context>.NextId++;
	}
}

internal static class TypedIdPool<Context> {
	internal static int NextId = 0;
}
