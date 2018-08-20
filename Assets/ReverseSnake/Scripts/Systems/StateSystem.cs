using Assets.ReverseSnake.Scripts.Extensions;
using LeopotamGroup.Ecs;
using System.Collections.Generic;

[EcsInject]
public class StateSystem : IEcsInitSystem, IEcsRunSystem
{
    EcsWorld _world = null;

    EcsFilterSingle<State> _state = null;

    EcsFilter<StateSetScoreEvent> _setScoreEvents = null;
    EcsFilter<StateAddStepsEvent> _addStepsEvents = null;
    EcsFilter<StateRemoveStepsEvent> _removeStepsEvent = null;

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

    }
}
