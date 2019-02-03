using Leopotam.Ecs;
using System.Collections.Generic;

sealed class StateRemoveTargetsEvent
{
    [EcsIgnoreNullCheck]
    public List<Target> Targets;
}
