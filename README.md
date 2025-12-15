# Yet Another ECS

Yet Another ECS (YAECS) is an entity-component-system (ECS) library made with the intent of having a minimal and non-prescriptive feature set.

It allows you to create and destroy entities, add and remove components from those entities, and retrieve entities which match a component type signature or value.

```
// Create a world.
var world = new World();

// Create an entity.
var entity = world.Create();

// Add a component to an entity.
entity.Set<Name>(new("Player"));

// Get a component from an entity.
var name = entity.Get<Name>().Value;

// Enumerate entities with a component type signature (filter).
var view = world.View(new Filter().Include<Name>().Exclude<Position>());

// Enumerate entities with a component value (index).
var view = world.View<Name>(new(name));
```

A component can be defined as a record struct, with the optional `[Indexed]` attribute to allow it to be used in views.

```
[Indexed] private record struct Name(string Value);

private record struct Position(Vector2 Value);

[Indexed] private record struct ChildOf(Entity Value);
```

Notably, an indexed component can be used to describe relations.

```
// Get the parent of an entity.
var parent = entity.Get<ChildOf>().Value;

// Get the children of an entity.
var children = world.View<ChildOf>(new(entity));
```