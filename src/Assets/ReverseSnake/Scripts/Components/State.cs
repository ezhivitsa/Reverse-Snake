using Leopotam.Ecs;
using System;
using System.Collections.Generic;

[Serializable]
sealed class State
{
    [EcsIgnoreNullCheck]
    public List<Target> Targets;

    [EcsIgnoreNullCheck]
    public List<Step> Steps;

    [EcsIgnoreNullCheck]
    public List<Wall> ActiveWalls;

    public int Score;
}