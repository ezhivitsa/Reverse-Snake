using Leopotam.Ecs;
using UnityEngine;
using UnityEngine.UI;

sealed class Score
{
    public int Amount;

    [EcsIgnoreNullCheck]
    public GameObject GameObject;

    [EcsIgnoreNullCheck]
    public GameObject UI;

    [EcsIgnoreNullCheck]
    public Text Result;

    public bool Silent = false;
}