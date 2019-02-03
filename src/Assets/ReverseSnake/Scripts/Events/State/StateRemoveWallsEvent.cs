using Leopotam.Ecs;
using System.Collections.Generic;

sealed class StateRemoveWallsEvent
{
    [EcsIgnoreNullCheck]
    public List<Wall> Walls;
}
