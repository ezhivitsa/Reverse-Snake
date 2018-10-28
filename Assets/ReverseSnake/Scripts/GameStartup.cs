using Assets.ReverseSnake.Scripts;
using Assets.ReverseSnake.Scripts.Managers;
using Leopotam.Ecs;
using Leopotam.Ecs.Ui.Systems;
using UnityEngine;

public class GameStartup : MonoBehaviour
{
    public static bool LoadState { get; set; }

    private StateManager _stateManager;

    ReverseSnakeWorld _world;
    EcsSystems _systems;

    [SerializeField]
    EcsUiEmitter _uiEmitter;

    void OnEnable ()
    {
        _world = new ReverseSnakeWorld();
    #if UNITY_EDITOR
        Leopotam.Ecs.UnityIntegration.EcsWorldObserver.Create(_world);
    #endif

        _systems = new EcsSystems(_world)
            .Add(_uiEmitter)
            .Add(new BoardElementsSystem())
            .Add(new StateSystem())
            .Add(new WallSystem())
            .Add(new StepSystem())
            .Add(new TargetSystem())
            .Add(new ScoreSystem())
            .Add(new UserInputSystem())
            .Add(new GameEndSystem())
            .Add(new EcsUiCleaner());

        _systems.Initialize();

    #if UNITY_EDITOR
        Leopotam.Ecs.UnityIntegration.EcsSystemsObserver.Create(_systems);
    #endif

        SaveState.OnLoaded += OnLoadState;
        if (LoadState)
        {
            SaveState.Load();
        }
    }

    void Update ()
    {
        _systems.Run();
    }

    void OnDisable ()
    {
        // destroy systems logical group.
        _systems.Dispose();
        _systems = null;
        // destroy world.
        _world.Dispose();
        _world = null;

        SaveState.OnLoaded -= OnLoadState;
    }

    void OnApplicationQuit()
    {
    }

    private void OnLoadState()
    {
        _stateManager = StateManager.GetInstance(_world);
        var state = SaveState.State;

        _stateManager.LoadFromState(state);
    }
}