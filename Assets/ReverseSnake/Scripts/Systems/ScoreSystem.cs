using Assets.src;
using LeopotamGroup.Ecs;
using UnityEngine;
using UnityEngine.UI;

[EcsInject]
public class ScoreSystem : IEcsRunSystem, IEcsInitSystem {
    EcsWorld _world = null;

    EcsFilter<Score> _scoreUiFilter = null;

    EcsFilter<ScoreChangeEvent> _scoreChangeFilter = null;

    void IEcsInitSystem.OnInitialize () {
        foreach (var ui in GameObject.FindGameObjectsWithTag(AppConstants.ScoreTag)) {
            var score = _world.CreateEntityWith<Score>();
            score.Amount = 0;
            score.Ui = ui.GetComponent<Text>();
            score.Ui.text = FormatText(score.Amount);
        }
    }

    void IEcsInitSystem.OnDestroy () { }

    string FormatText (int v) {
        return string.Format ("Score: {0}", v);
    }

    void IEcsRunSystem.OnUpdate () {
        for (var i = 0; i < _scoreChangeFilter.EntitiesCount; i++) {
            var amount = _scoreChangeFilter.Components1[i].Amount;
            for (var j = 0; j < _scoreUiFilter.EntitiesCount; j++) {
                var score = _scoreUiFilter.Components1[j];
                score.Amount += amount;
                score.Ui.text = FormatText (score.Amount);
            }
            _world.RemoveEntity(_scoreChangeFilter.Entities[i]);
        }
    }
}