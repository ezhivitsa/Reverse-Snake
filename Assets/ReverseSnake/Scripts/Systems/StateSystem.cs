using Assets.ReverseSnake.Scripts;
using Assets.ReverseSnake.Scripts.Extensions;
using Leopotam.Ecs;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[EcsInject]
public class StateSystem : IEcsPreInitSystem, IEcsRunSystem
{
    EcsWorld _world = null;

    EcsFilterSingle<State> _state = null;

    EcsFilter<StateSetScoreEvent> _setScoreEvents = null;
    EcsFilter<StateAddStepsEvent> _addStepsEvents = null;
    EcsFilter<StateRemoveStepsEvent> _removeStepsEvent = null;
    EcsFilter<StateAddTargetsEvent> _addTargetsEvents = null;
    EcsFilter<StateRemoveTargetsEvent> _removeTargetsEvents = null;
    EcsFilter<StateAddWallsEvent> _addWallsEvents = null;
    EcsFilter<StateRemoveWallsEvent> _removeWallsEvents = null;
    EcsFilter<StateClearEvent> _clearEvents = null;
    EcsFilter<StateLoadEvent> _loadEvents = null;

    public void PreInitialize()
    {
        _state.Data.Targets = new List<Target>();
        _state.Data.Steps = new List<Step>();
        _state.Data.ActiveWalls = new List<Wall>();
        _state.Data.Score = 0;
    }

    public void Run()
    {
        HandleSetScoreEvent();

        HandleAddStepsEvent();
        HandleAddTargetsEvent();
        HandleAddWallsEvent();

        HandleRemoveStepsEvent();
        HandleRemoveTargetsEvent();
        HandleRemoveWallsEvent();

        HandleClearEvent();
        HandleLoadEvent();
    }

    public void PreDestroy()
    {
        SaveState.Save(_state.Data);
    }

    private void HandleSetScoreEvent()
    {
        _setScoreEvents.HandleEvents(_world, (eventData) => {
            _state.Data.Score = eventData.Score;
        });
    }

    private void HandleAddStepsEvent()
    {
        _addStepsEvents.HandleEvents(_world, (eventData) => {
            _state.Data.Steps.AddRange(eventData.Steps);
        });
    }

    private void HandleRemoveStepsEvent()
    {
        _removeStepsEvent.HandleEvents(_world, (eventData) => {
            _state.Data.Steps = _state.Data.Steps
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
            _state.Data.Targets.AddRange(eventData.Targets);
        });
    }

    private void HandleRemoveTargetsEvent()
    {
        _removeTargetsEvents.HandleEvents(_world, (eventData) => {
            _state.Data.Targets = _state.Data.Targets
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
            _state.Data.ActiveWalls.AddRange(eventData.Walls);
        });
    }

    private void HandleRemoveWallsEvent()
    {
        _removeWallsEvents.HandleEvents(_world, (eventData) => {
            _state.Data.ActiveWalls = _state.Data.ActiveWalls
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
            _state.Data.Targets.Clear();
            _state.Data.Steps.Clear();
            _state.Data.ActiveWalls.Clear();
            _state.Data.Score = 0;
        });
    }

    private void HandleLoadEvent()
    {
        _loadEvents.HandleEvents(_world, (eventData) => {
            _state.Data.Targets = eventData.State.Targets;
            _state.Data.Steps = eventData.State.Steps;
            _state.Data.ActiveWalls = eventData.State.ActiveWalls;
            _state.Data.Score = eventData.State.Score;
        });
    }
}
