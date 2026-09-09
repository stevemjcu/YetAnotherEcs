# Yet Another ECS

Yet Another ECS (YAECS) is an entity-component-system (ECS) library made with the intent of having a minimal and non-prescriptive feature set.

It allows you to create and destroy entities, add and remove components from those entities, and retrieve entities with a specific component structure or value.

```
// Create a world.
var world = new World();

// Create an entity.
var entity = world.Create();

// Add a component to an entity.
entity.Set<Name>(new("Player"));

// Get a component from an entity.
var name = entity.Get<Name>().Value;

// Enumerate entities with a component structure.
var view = world.Query(new Filter().Include<Name>().Exclude<Position>());

// Enumerate entities with a component value.
var view = world.Query<Name>(new(name));
```

A component can be defined as a struct.

```
private record struct Name(string Value);

private record struct Position(Vector2 Value);

private record struct ChildOf(Entity Value);
```

A component can also describe relations.

```
// Get the parent of an entity.
var parent = entity.Get<ChildOf>().Value;

// Get the children of an entity.
var children = world.Query<ChildOf>(new(entity));
```