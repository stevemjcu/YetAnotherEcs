namespace YetAnotherEcs;

/// <summary>
/// Manages the storage for a component type.
/// </summary>
internal class ComponentStore<T>
{
	private static class TypedIdAssigner
	{
		internal static IdAssigner IdAssigner = new();
	}

	public readonly static int Id = TypedIdAssigner.IdAssigner.Assign();

	private readonly Dictionary<int, T> ComponentByEntityId = [];

	public void Set(int id, T component) => ComponentByEntityId[id] = component;
}