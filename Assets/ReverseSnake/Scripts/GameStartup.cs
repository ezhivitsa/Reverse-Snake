using Assets.ReverseSnake.Scripts;
using Assets.ReverseSnake.Scripts.Managers;
using Assets.ReverseSnake.Scripts.Systems;
using Leopotam.Ecs;
using Leopotam.Ecs.Ui.Systems;
using UnityEngine;

public class GameStartup : MonoBehaviour
{
    public static bool LoadState { get; set; }

    private StateManager _stateManager;
    private GameManager _gameManager;

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

        _gameManager = GameManager.GetInstance(_world);

        _systems = new EcsSystems(_world)
            .Add(_uiEmitter)
            .Add(new StateSystem())
            .Add(new StepReactiveSystemOnAdd())
            .Add(new StepReactiveSystemOnRemove())
            .Add(new WallSystem())
            .Add(new WallReactivitySystemOnAdd())
            .Add(new WallReactivitySystemOnRemove())
            .Add(new TargetReactivitySystemOnAdd())
            .Add(new TargetReactivitySystemOnUpdate())
            .Add(new TargetReactiveSystemOnRemove())
            .Add(new ScoreReactivitySystemOnUpdate())
            .Add(new UserInputSystem())
            .Add(new GameEndSystem());

        _systems.Initialize();

    #if UNITY_EDITOR
        Leopotam.Ecs.UnityIntegration.EcsSystemsObserver.Create(_systems);
    #endif

        SaveState.OnLoaded += OnLoadState;
        if (LoadState)
        {
            SaveState.Load();
        }
        else
        {
            _gameManager.StartNewGame();
        }
    }

    void Update ()
    {
        if (_systems != null)
        {
            // Process systems.
            _systems.Run();
            // Important: automatic clearing one-frame components (ui-events).
            _world.RemoveOneFrameComponents();
        }
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