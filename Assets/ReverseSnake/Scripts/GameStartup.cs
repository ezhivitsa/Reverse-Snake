using LeopotamGroup.Ecs;
using UnityEngine;

public class GameStartup : MonoBehaviour {
    EcsWorld _world;

    EcsSystems _systems;

    void OnEnable () {
        _world = new EcsWorld ();
    #if UNITY_EDITOR
        LeopotamGroup.Ecs.UnityIntegration.EcsWorldObserver.Create(_world);
    #endif

        _systems = new EcsSystems(_world)
            .Add(new BoardElementSystem())
            .Add(new WallSystem())
            .Add(new StepSystem())
            .Add(new TargetSystem())
            .Add(new ScoreSystem())
            .Add(new UserInputSystem());

        _systems.OnInitialize();

    #if UNITY_EDITOR
        LeopotamGroup.Ecs.UnityIntegration.EcsSystemsObserver.Create (_systems);
    #endif
    }

    void Update () {
        _systems.Run();
    }

    void OnDisable () {
        _systems.OnDestroy ();
    }
}