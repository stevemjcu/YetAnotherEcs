namespace YetAnotherEcs;

[AttributeUsage(AttributeTargets.Struct)]
public class ComponentAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Struct)]
public class IndexedComponentAttribute : Attribute
{
}
