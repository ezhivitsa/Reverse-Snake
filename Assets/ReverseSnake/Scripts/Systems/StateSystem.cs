using Assets.ReverseSnake.Scripts.Extensions;
using LeopotamGroup.Ecs;
using System.Collections.Generic;
using System.Linq;

[EcsInject]
public class StateSystem : IEcsInitSystem, IEcsRunSystem
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

    void IEcsInitSystem.OnInitialize()
    {
        _state.Data.Targets = new List<Target>();
        _state.Data.Steps = new List<Step>();
        _state.Data.ActiveWalls = new List<Wall>();
        _state.Data.Score = 0;
    }

    void IEcsRunSystem.OnUpdate()
    {
        HandleSetScoreEvent();

        HandleAddStepsEvent();
        HandleAddTargetsEvent();
        HandleAddWallsEvent();

        HandleRemoveStepsEvent();
        HandleRemoveTargetsEvent();
        HandleRemoveWallsEvent();
    }

    void IEcsInitSystem.OnDestroy() { }

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
}
