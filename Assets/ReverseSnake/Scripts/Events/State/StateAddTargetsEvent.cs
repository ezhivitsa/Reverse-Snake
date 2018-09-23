using Leopotam.Ecs;
using System.Collections.Generic;

sealed class StateAddTargetsEvent
{
    [EcsIgnoreNullCheck]
    public List<Target> Targets;
}
