using YetAnotherEcs.General;

namespace YetAnotherEcs;

public interface IComponent
{
	public static int GetId<T>() where T : struct, IComponent
	{
		return TypedIdPool<IComponent, T>.Id;
	}

	public static int GetBitmask<T>() where T : struct, IComponent
	{
		return 1 << GetId<T>();
	}
}
