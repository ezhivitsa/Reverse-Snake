using Leopotam.Ecs;
using System.Collections.Generic;

sealed class StateAddWallsEvent
{
    [EcsIgnoreNullCheck]
    public List<Wall> Walls;
}
