using Leopotam.Ecs;
using System.Collections.Generic;

sealed class CreateWallsEvent
{
    [EcsIgnoreNullCheck]
    public List<Wall> Walls;
}
