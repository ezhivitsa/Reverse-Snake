using Leopotam.Ecs;

sealed class StateLoadEvent
{
    [EcsIgnoreNullCheck]
    public State State;
}
