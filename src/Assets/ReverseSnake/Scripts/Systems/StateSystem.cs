using Assets.ReverseSnake.Scripts;
using Assets.ReverseSnake.Scripts.Extensions;
using Leopotam.Ecs;
using System.Linq;

[EcsInject]
public class StateSystem : IEcsRunSystem
{
    ReverseSnakeWorld _world = null;

    EcsFilter<StateSetScoreEvent> _setScoreEvents = null;
    EcsFilter<StateAddStepsEvent> _addStepsEvents = null;
    EcsFilter<StateRemoveStepsEvent> _removeStepsEvent = null;
    EcsFilter<StateAddTargetsEvent> _addTargetsEvents = null;
    EcsFilter<StateRemoveTargetsEvent> _removeTargetsEvents = null;
    EcsFilter<StateAddWallsEvent> _addWallsEvents = null;
    EcsFilter<StateRemoveWallsEvent> _removeWallsEvents = null;
    EcsFilter<StateClearEvent> _clearEvents = null;
    EcsFilter<StateLoadEvent> _loadEvents = null;

    public void Run()
    {
        var hasEvents = !_setScoreEvents.IsEmpty() ||
            !_addStepsEvents.IsEmpty() ||
            !_removeStepsEvent.IsEmpty() ||
            !_addTargetsEvents.IsEmpty() ||
            !_removeTargetsEvents.IsEmpty() ||
            !_addWallsEvents.IsEmpty() ||
            !_removeWallsEvents.IsEmpty();

        HandleSetScoreEvent();

        HandleAddStepsEvent();
        HandleAddTargetsEvent();
        HandleAddWallsEvent();

        HandleRemoveStepsEvent();
        HandleRemoveTargetsEvent();
        HandleRemoveWallsEvent();

        HandleClearEvent();
        HandleLoadEvent();

        if (hasEvents)
        {
            SaveState.State = _world.State;

            // ToDo: find a way to save only on exit
            SaveState.Save(_world.State);
        }
    }

    private void HandleSetScoreEvent()
    {
        _setScoreEvents.HandleEvents(_world, (eventData) => {
            _world.State.Score = eventData.Score;
        });
    }

    private void HandleAddStepsEvent()
    {
        _addStepsEvents.HandleEvents(_world, (eventData) => {
            _world.State.Steps.AddRange(eventData.Steps);
        });
    }

    private void HandleRemoveStepsEvent()
    {
        _removeStepsEvent.HandleEvents(_world, (eventData) => {
            _world.State.Steps = _world.State.Steps
                .Where((step) => {
                    var stepToRemove = eventData.Steps.Find(s =>
                    {
                        return s.Column == step.Column &&
                            s.Row == step.Row &&
                            s.Round == step.Round &&
                            s.Number == step.Number;
                    });
                    return stepToRemove == null;
                })
                .ToList();
        });
    }

    private void HandleAddTargetsEvent()
    {
        _addTargetsEvents.HandleEvents(_world, (eventData) => {
            _world.State.Targets.AddRange(eventData.Targets);
        });
    }

    private void HandleRemoveTargetsEvent()
    {
        _removeTargetsEvents.HandleEvents(_world, (eventData) => {
            _world.State.Targets = _world.State.Targets
                .Where((target) => {
                    return !eventData.Targets.Any((t) =>
                    {
                        return t.Column == target.Column &&
                            t.Row == target.Row &&
                            t.Round == target.Round &&
                            t.Value == target.Value;
                    });
                })
                .ToList();
        });
    }

    private void HandleAddWallsEvent()
    {
        _addWallsEvents.HandleEvents(_world, (eventData) => {
            _world.State.ActiveWalls.AddRange(eventData.Walls);
        });
    }

    private void HandleRemoveWallsEvent()
    {
        _removeWallsEvents.HandleEvents(_world, (eventData) => {
            _world.State.ActiveWalls = _world.State.ActiveWalls
                .Where((wall) => {
                    return !eventData.Walls.Any((w) =>
                    {
                        return w.Column == wall.Column &&
                            w.Row == wall.Row &&
                            w.Direction == wall.Direction;
                    });
                })
                .ToList();
        });
    }

    private void HandleClearEvent()
    {
        _clearEvents.HandleEvents(_world, (eventData) => {
            _world.State.Targets.Clear();
            _world.State.Steps.Clear();
            _world.State.ActiveWalls.Clear();
            _world.State.Score = 0;
        });
    }

    private void HandleLoadEvent()
    {
        _loadEvents.HandleEvents(_world, (eventData) => {
            _world.State.Targets = eventData.State.Targets;
            _world.State.Steps = eventData.State.Steps;
            _world.State.ActiveWalls = eventData.State.ActiveWalls;
            _world.State.Score = eventData.State.Score;
        });
    }
}
