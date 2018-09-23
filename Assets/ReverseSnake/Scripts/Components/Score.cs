using Leopotam.Ecs;
using UnityEngine;
using UnityEngine.UI;

sealed class Score
{
    public int Amount;

    [EcsIgnoreNullCheck]
    public GameObject GameObject;

    [EcsIgnoreNullCheck]
    public Text Ui;
}