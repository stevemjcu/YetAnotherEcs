using System.Runtime.CompilerServices;

namespace YetAnotherEcs.Storage;

/// <summary>
/// Encapsulates the storage for all component types.
/// </summary>
internal class ComponentStore
{
	private int NextTypeId = 0;
	private readonly Dictionary<Type, int> TypeIdByType = [];
	private readonly List<object> StoreByTypeId = [];

	public void Set<T>(int id, T component) where T : struct =>
		Store<T>()[id] = component;

	public void Remove<T>(int id) where T : struct =>
		Store<T>().Remove(id);

	public T Get<T>(int id) where T : struct =>
		Store<T>()[id];

	public int Get<T>() where T : struct =>
		Store<T>().Keys.Single(); // Does this allocate?

	// TODO: Profile to determine if a static ID would be much better.
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int Id<T>() where T : struct =>
		TypeIdByType[Type<T>()];

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private Dictionary<int, T> Store<T>() where T : struct =>
		(Dictionary<int, T>)StoreByTypeId[Id<T>()];

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private Type Type<T>() where T : struct
	{
		var type = typeof(T);

		if (!TypeIdByType.ContainsKey(type))
		{
			TypeIdByType[type] = NextTypeId++;
			StoreByTypeId.Add(new Dictionary<int, T>());
		}

		return type;
	}
}