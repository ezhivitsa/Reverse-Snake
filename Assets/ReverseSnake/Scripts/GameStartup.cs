using Assets.ReverseSnake.Scripts;
using Assets.ReverseSnake.Scripts.Managers;
using LeopotamGroup.Ecs;
using UnityEngine;

public class GameStartup : MonoBehaviour
{
    public static bool LoadState { get; set; }

    private StateManager _stateManager;

    EcsWorld _world;
    EcsSystems _systems;

    void OnEnable ()
    {
        _world = new EcsWorld();
    #if UNITY_EDITOR
        LeopotamGroup.Ecs.UnityIntegration.EcsWorldObserver.Create(_world);
    #endif

        EcsFilterSingle<BoardElements>.Create(_world);
        EcsFilterSingle<State>.Create(_world);

        _systems = new EcsSystems(_world)
            .Add(new BoardElementsSystem())
            .Add(new StateSystem())
            .Add(new WallSystem())
            .Add(new StepSystem())
            .Add(new TargetSystem())
            .Add(new ScoreSystem())
            .Add(new UserInputSystem())
            .Add(new GameEndSystem());

        _systems.OnInitialize();

    #if UNITY_EDITOR
        LeopotamGroup.Ecs.UnityIntegration.EcsSystemsObserver.Create(_systems);
    #endif

        SaveState.OnLoaded += OnLoadState;
        if (LoadState)
        {
            SaveState.Load();
        }
    }

    void Update ()
    {
        _systems.OnUpdate();
    }

    void OnDisable ()
    {
        _world.Dispose();
        _systems = null;
        _world = null;

        SaveState.OnLoaded -= OnLoadState;
    }

    private void OnLoadState()
    {
        _stateManager = StateManager.GetInstance(_world);
        var state = SaveState.State;

        _stateManager.LoadFromState(state);
    }
}