using Leopotam.Ecs;
using System.Collections.Generic;

sealed class StateAddStepsEvent
{
    [EcsIgnoreNullCheck]
    public List<Step> Steps;
}
