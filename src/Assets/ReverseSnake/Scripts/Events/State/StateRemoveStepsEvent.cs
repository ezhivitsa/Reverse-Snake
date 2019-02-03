using Leopotam.Ecs;
using System.Collections.Generic;

sealed class StateRemoveStepsEvent
{
    [EcsIgnoreNullCheck]
    public List<Step> Steps;
}
