using System;
using System.Collections.Generic;

[Serializable]
sealed class State
{
    public List<Target> Targets;

    public List<Step> Steps;

    public List<Wall> ActiveWalls;

    public int Score;
}