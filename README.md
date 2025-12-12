# Yet Another ECS

Yet Another ECS (YAECS) is an entity-component-system (ECS) library made with the intent of having a minimal and non-prescriptive feature set.

It allows you to create and destroy entities, add and remove components from those entities, and retrieve entities which match a filter or index.

```
// Create a world.
var world = new World();

// Create an entity.
var id = world.Create();

// Add a component to an entity.
world.Add(id, new Name("Player"));

// Get a component from an entity.
var name = world.Get<Name>(id);

// View entities with a component type (filter).
var view = world.View(new Filter().Include<Name>());

// View entities with a component value (index).
var view = world.View(name);
```

A component can be defined as a record struct, with the optional `[Indexed]` attribute to allow it to be used in views.

```
[Indexed] private record struct Name(string Value);

private record struct Position(int X, int Y);

[Indexed] private record struct ChildOf(int Id);
```

Notably, an indexed component can be used to describe relations.

```
var parent = world.Get<ChildOf>(id);
var children = world.View(new ChildOf(id));
```