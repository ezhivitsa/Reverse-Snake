using Assets.ReverseSnake.Scripts.Extensions;
using Assets.ReverseSnake.Scripts.Managers;
using Assets.src;
using Leopotam.Ecs;
using UnityEngine;
using UnityEngine.UI;

[EcsInject]
public class ScoreSystem : IEcsRunSystem, IEcsInitSystem
{
    EcsWorld _world = null;

    EcsFilter<Score> _scoreUiFilter = null;

    EcsFilter<ScoreChangeEvent> _scoreChangeFilter = null;
    EcsFilter<ScoreSetEvent> _scoreSetFilter = null;

    private StateManager _stateManager = null;

    public void Initialize()
    {
        _stateManager = StateManager.GetInstance(_world);

        foreach (var ui in GameObject.FindGameObjectsWithTag(AppConstants.ScoreTag))
        {
            var score = _world.CreateEntityWith<Score>();
            score.Amount = 0;
            score.GameObject = ui;
            score.Ui = ui.GetComponent<Text>();
            score.Ui.text = FormatText(score.Amount);
        }

        if (!GameStartup.LoadState) {
            _stateManager.SetScore(0);
        }
    }

    public void Destroy()
    {
        _scoreUiFilter.ToEntitieNumbersList().ForEach(entity => {
            _world.RemoveEntity(entity);
        });
    }

    public void Run()
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
                score.Amount += amount;
                score.Ui.text = FormatText(score.Amount);

                _stateManager.SetScore(score.Amount);
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
                score.Amount = amount;
                score.Ui.text = FormatText(score.Amount);
            });

            if (!scoreEvent.Silent)
            {
                _stateManager.SetScore(amount);
            }
        });
    }

    private string FormatText(int v)
    {
        return string.Format("{0}", v);
    }
}