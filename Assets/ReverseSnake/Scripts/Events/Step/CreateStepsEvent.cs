using Leopotam.Ecs;
using System.Collections.Generic;

sealed class CreateStepsEvent
{
    [EcsIgnoreNullCheck]
    public List<Step> Steps;
}
