using System.Runtime.CompilerServices;

namespace YetAnotherEcs.Storage;

/// <summary>
/// Encapsulates the storage for all components.
/// </summary>
internal class ComponentStore
{
	private int NextTypeId = 0;
	private readonly Dictionary<Type, int> TypeIdByType = [];
	private readonly List<object> StoreByTypeId = [];

	// TODO: Profile to determine if a static ID would be much better.
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int Id<T>() where T : struct =>
		TypeIdByType[GetType<T>()];

	public void Set<T>(int id, T component) where T : struct =>
		GetStore<T>()[id] = component;

	public void Remove<T>(int id) where T : struct =>
		GetStore<T>().Remove(id);

	public T Get<T>(int id) where T : struct =>
		GetStore<T>()[id];

	public int Get<T>() where T : struct =>
		GetStore<T>().Keys.Single(); // Does this allocate?

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private Dictionary<int, T> GetStore<T>() where T : struct =>
		(Dictionary<int, T>)StoreByTypeId[Id<T>()];

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private Type GetType<T>() where T : struct
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