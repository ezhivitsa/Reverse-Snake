using Assets.ReverseSnake.Scripts.Extensions;
using Assets.src;
using LeopotamGroup.Ecs;
using UnityEngine;
using UnityEngine.UI;

[EcsInject]
public class ScoreSystem : IEcsRunSystem, IEcsInitSystem {
    EcsWorld _world = null;

    EcsFilter<Score> _scoreUiFilter = null;

    EcsFilter<ScoreChangeEvent> _scoreChangeFilter = null;
    EcsFilter<ScoreSetEvent> _scoreSetFilter = null;

    void IEcsInitSystem.OnInitialize()
    {
        foreach (var ui in GameObject.FindGameObjectsWithTag(AppConstants.ScoreTag))
        {
            var score = _world.CreateEntityWith<Score>();
            score.Amount = 0;
            score.GameObject = ui;
            score.Ui = ui.GetComponent<Text>();
            score.Ui.text = FormatText(score.Amount);
        }
    }

    void IEcsInitSystem.OnDestroy()
    {
    }

    void IEcsRunSystem.OnUpdate()
    {
        HandleChangeEvent();
        HandleSetEvent();
    }

    private void HandleChangeEvent()
    {
        _scoreChangeFilter.HandleEvents(_world, (scoreEvent) =>
        {
            var amount = scoreEvent.Amount;
            _scoreUiFilter.ToEntitiesList().ForEach((score) =>
            {
                score.Amount = amount;
                score.Ui.text = FormatText(score.Amount);
            });
        });
    }

    private void HandleSetEvent()
    {
        _scoreSetFilter.HandleEvents(_world, (scoreEvent) =>
        {
            var amount = scoreEvent.Amount;
            _scoreUiFilter.ToEntitiesList().ForEach((score) =>
            {
                score.Amount += amount;
                score.Ui.text = FormatText(score.Amount);
            });
        });
    }

    private string FormatText(int v)
    {
        return string.Format("Score: {0}", v);
    }
}