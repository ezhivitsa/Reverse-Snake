[![gitter](https://img.shields.io/gitter/room/leopotam/ecs.svg)](https://gitter.im/leopotam/ecs)
[![license](https://img.shields.io/github/license/Leopotam/ecs-reactive.svg)](https://github.com/Leopotam/ecs-reactive/blob/develop/LICENSE)
# Reactive behaviour for LeoECS
Reactive filters / systems for using with [Entity Component System Framework](https://github.com/Leopotam/ecs).

> Tested on unity 2018.3 (not dependent on it) and contains assembly definition for compiling to separate assembly file for performance reason.

> **Its early work-in-progress stage, not recommended to use in real projects, any api / behaviour can change later.**

> **Important!** All reacted entities are just cached results: real entities / components already can be removed from world / component! If you know that you use similar behaviour (entity can be removed before reactive system starts to work) - `EcsWorld.IsEntityExists` method should be used at reactive system processing per each entity. But better to not remove entities before reactive systems.

## OnAdd / OnRemove example:
```csharp
class ReactiveComponent1 {
    public int Id;
}

sealed class TestReactiveStartup : MonoBehaviour {
    EcsWorld _world;
    EcsSystems _systems;

    void OnEnable () {
        _world = new EcsWorld ();
        _systems = new EcsSystems (_world);
        _systems
            .Add (new TestReactiveSystemOnAdd ())
            .Add (new TestReactiveSystemOnRemove ())
            .Initialize ();
    }

    void Update () {
        _systems.Run ();
    }

    void OnDisable () {
        _systems.Dispose ();
        _systems = null;
        _world.Dispose ();
        _world = null;
    }
}

[EcsInject]
sealed class TestReactiveSystemOnAdd : EcsReactiveSystem<ReactiveComponent1>, IEcsInitSystem {
    EcsWorld _world = null;

    void IEcsInitSystem.Initialize () {
        // create test data for catching OnAdded event at react system later.
        _world.CreateEntityWith<ReactiveComponent1> ().Id = 10;
        _world.CreateEntityWith<ReactiveComponent1> ().Id = 20;
        _world.CreateEntityWith<ReactiveComponent1> ().Id = 30;
    }

    void IEcsInitSystem.Destroy () { }

    protected override EcsReactiveType GetReactiveType () {
        // this system should react on Add event.
        return EcsReactiveType.OnAdded;
    }

    // this method will be called only if there are any entities for processing.
    protected override void RunReactive () {
        // ReactedEntitiesCount contains amount of reacted entities at ReactedEntities collection.
        for (var i = 0; i < ReactedEntitiesCount; i++) {
            var entity = ReactedEntities[i];
            var c = _world.GetComponent<ReactiveComponent1> (entity);
            Debug.LogFormat ("[ON-ADDED] Reacted entity \"{0}\" and component {1}", entity, c.Id);

            // remove reacted entities for test OnRemoved reactive system.
            _world.RemoveEntity (entity);
        }
    }
}

[EcsInject]
sealed class TestReactiveSystemOnRemove : EcsReactiveSystem<ReactiveComponent1> {
    protected override EcsReactiveType GetReactiveType () {
        // this system should react on Remove event.
        return EcsReactiveType.OnRemoved;
    }

    // this method will be called only if there are any entities for processing.
    protected override void RunReactive () {
        for (var i = 0; i < ReactedEntitiesCount; i++) {
            Debug.LogFormat ("[ON-REMOVE] Reacted entity: {0}", ReactedEntities[i]);
        }
    }
}
```

Result log:
```
[ON-ADDED] Reacted entity "0" and component 10
[ON-ADDED] Reacted entity "1" and component 20
[ON-ADDED] Reacted entity "2" and component 30
[ON-REMOVE] Reacted entity: 0
[ON-REMOVE] Reacted entity: 1
[ON-REMOVE] Reacted entity: 2
```

## MarkComponentAsUpdated example:
```csharp
class UpdateComponent1 {
    public int Id;
}

sealed class TestUpdateReactiveStartup : MonoBehaviour {
    EcsWorld _world;
    EcsSystems _systems;

    void OnEnable () {
        _world = new EcsWorld ();
        _systems = new EcsSystems (_world);
        _systems
            .Add (new TestRunSystem ())
            .Add (new TestReactiveSystemOnUpdate ())
            .Initialize ();
    }

    void Update () {
        _systems.Run ();
    }

    void OnDisable () {
        _systems.Dispose ();
        _systems = null;
        _world.Dispose ();
        _world = null;
    }
}

[EcsInject]
sealed class TestRunSystem : IEcsInitSystem, IEcsRunSystem {
    EcsWorld _world = null;
    EcsFilter<UpdateComponent1> _filter = null;

    void IEcsInitSystem.Initialize () {
        _world.CreateEntityWith<UpdateComponent1> ().Id = 10;
    }

    void IEcsInitSystem.Destroy () { }

    void IEcsRunSystem.Run () {
        for (var i = 0; i < _filter.EntitiesCount; i++) {
            _filter.Components1[i].Id++;
            // Important! This method should be called for each component for processing at EcsUpdateReactiveSystem.
            _world.MarkComponentAsUpdated<UpdateComponent1> (_filter.Entities[i]);
        }
    }
}

[EcsInject]
sealed class TestReactiveSystemOnUpdate : EcsUpdateReactiveSystem<UpdateComponent1> {
    // this method will be called only if there are any entities for processing.
    protected override void RunUpdateReactive () {
        for (var i = 0; i < ReactedEntitiesCount; i++) {
            var entity = ReactedEntities[i];
            var c = _world.GetComponent<UpdateComponent1> (entity);
            Debug.LogFormat ("[ON-UPDATE] Updated entity: {0}, new value: {1}", entity, c.Id);
        }
    }
}
```

Result log:
```
[ON-UPDATE] Updated entity: 0, new value: 11
[ON-UPDATE] Updated entity: 0, new value: 12
[ON-UPDATE] Updated entity: 0, new value: 13
[ON-UPDATE] Updated entity: 0, new value: 14
...
```

# License
The software released under the terms of the [MIT license](./LICENSE). Enjoy.

# Donate
Its free opensource software, but you can buy me a coffee:

<a href="https://www.buymeacoffee.com/leopotam" target="_blank"><img src="https://www.buymeacoffee.com/assets/img/custom_images/yellow_img.png" alt="Buy Me A Coffee" style="height: auto !important;width: auto !important;" ></a>