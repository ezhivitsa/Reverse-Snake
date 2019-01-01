[![gitter](https://img.shields.io/gitter/room/leopotam/ecs.svg)](https://gitter.im/leopotam/ecs)
[![license](https://img.shields.io/github/license/Leopotam/ecs-ui.svg)](https://github.com/Leopotam/ecs-ui/blob/develop/LICENSE)
# Unity uGui extension for Entity Component System framework
Easy bindings for events from Unity uGui to [ECS framework](https://github.com/Leopotam/ecs) - main goal of this extension.

> Tested on unity 2018.3 (dependent on Unity engine) and contains assembly definition for compiling to separate assembly file for performance reason.

> Dependent on [ECS framework](https://github.com/Leopotam/ecs) - ECS framework should be imported to unity project first.

# Systems

## EcsUiEmitter

Ecs run-system that generates entities with events data to `ecs world`. Should be placed on root GameObject of Ui hierarchy in scene and connected in `ecs world` before any systems that should process events from ui:
```csharp
public class Startup : MonoBehaviour {
    // Field that should be initialized by instance of `EcsUiEmitter` assigned to Ui root GameObject.
    [SerializeField]
    EcsUiEmitter _uiEmitter;

    EcsSystems _systems;

    void Start () {
        var world = new EcsWorld ();
        _systems = new EcsSystems(world)
            .Add (_uiEmitter);
            // Additional initialization here...
        _systems.Initialize ();
    }
}
```

# Actions
MonoBehaviour components that should be added to uGui widgets to transfer events from them to `ecs-world` (`EcsUiClickAction`, `EcsUiDragAction` and others). Each action component contains reference to `EcsUiEmitter` in scene (if not inited - will try to find emitter automatically) and logical name `WidgetName` that can helps to detect source of event inside ecs-system.

# Components
Event data containers: `EcsUiClickEvent`, `EcsUiBeginDragEvent`, `EcsUiEndDragEvent` and others - they can be used as ecs-components with standard filtering through `EcsFilter`:
```csharp
[EcsInject]
public class TestUiClickEventSystem : IEcsRunSystem {
    EcsWorld _world = null;

    EcsFilter<EcsUiClickEvent> _clickEvents = null;

    void IEcsRunSystem.Run () {
        for (var i = 0; i < _clickEvents.EntitiesCount; i++) {
            EcsUiClickEvent data = _clickEvents.Components1[i];
            Debug.Log ("Im clicked!", data.Sender);
        }
    }
}
```

# Initialization
```csharp
public class Startup : MonoBehaviour {
    // Field that should be initialized by instance of `EcsUiEmitter` assigned to Ui root GameObject.
    [SerializeField]
    EcsUiEmitter _uiEmitter;

    EcsSystems _systems;

    IEnumerator Start () {
        // For correct registering named widgets at EcsUiEmitter.
        yield return null;
        var world = new EcsWorld ();
        _systems = new EcsSystems(world);
        _systems
            .Add (_uiEmitter)
            // Additional systems here...
            .Initialize ();
    }

    void Update () {
        if (_systems != null) {
            // Process systems.
            _systems.Run();
            // Important: automatic clearing one-frame components (ui-events).
            _world.RemoveOneFrameComponents ();
        }
    }

    void OnDisable () {
        _systems.Dispose ();
        _systems = null;
        _world.Dispose ();
        _world = null;
    }
}
```

# Examples
[Examples repo](https://github.com/Leopotam/ecs-ui.examples.git).

# License
The software released under the terms of the [MIT license](./LICENSE). Enjoy.

# Donate
Its free opensource software, but you can buy me a coffee:

<a href="https://www.buymeacoffee.com/leopotam" target="_blank"><img src="https://www.buymeacoffee.com/assets/img/custom_images/yellow_img.png" alt="Buy Me A Coffee" style="height: auto !important;width: auto !important;" ></a>