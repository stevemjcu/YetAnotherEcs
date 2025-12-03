namespace YetAnotherEcs.Alt;

public class World
{
	#region Entity

	public int Create(int id = -1) => throw new NotImplementedException();

	public int Recycle(int id) => throw new NotImplementedException();

	public bool Exists(int id) => throw new NotImplementedException();

	#endregion

	#region Component

	public void Set<T>(int id, T value) where T : struct => throw new NotImplementedException();

	public void Remove<T>(int id, T value) where T : struct => throw new NotImplementedException();

	public void Has<T>(int id) where T : struct => throw new NotImplementedException();

	public T Get<T>() where T : struct => throw new NotImplementedException();

	#endregion

	#region Query

	public Filter Filter() => throw new NotImplementedException();

	public void Index<T>() => throw new NotImplementedException();

	#endregion

	#region Enumerate

	public int Single<T>() => throw new NotImplementedException();

	public int Single<T>(T index) => throw new NotImplementedException();

	public IReadOnlySet<int> Query(Filter filter) => throw new NotImplementedException();

	public IReadOnlySet<int> Query<T>(T index) => throw new NotImplementedException();

	#endregion
}
