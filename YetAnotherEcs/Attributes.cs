namespace YetAnotherEcs;

/// <summary>
/// Represents a component.
/// </summary>
[AttributeUsage(AttributeTargets.Struct)]
public class ComponentAttribute : Attribute
{
}

/// <summary>
/// Represents an index(ed component).
/// </summary>
[AttributeUsage(AttributeTargets.Struct)]
public class IndexedAttribute : Attribute
{
}
