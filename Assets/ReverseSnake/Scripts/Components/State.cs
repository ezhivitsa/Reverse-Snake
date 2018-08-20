using System.Collections.Generic;

sealed class State
{
    public List<Target> Targets;

    public List<Step> Steps;

    public List<Wall> ActiveWalls;

    public int Score;
}