using Assets.ReverseSnake.Scripts.Extensions;
using Assets.ReverseSnake.Scripts.Helpers;
using Assets.ReverseSnake.Scripts.Managers;
using Assets.src;
using Leopotam.Ecs;
using Leopotam.Ecs.Reactive;
using UnityEngine;

namespace Assets.ReverseSnake.Scripts.Systems
{
    [EcsInject]
    sealed class TargetReactivitySystemOnAdd : EcsReactiveSystem<Target>, IEcsInitSystem
    {
        ReverseSnakeWorld _world = null;

        const string DefaultTargetPath = "Objects/DefaultTarget";
        const string AddTailRemoveTwoWallTarget = "Objects/AddTailRemoveTwoWallTarget";
        const string RemoveTailAddWallTarget = "Objects/RemoveTailAddWallTarget";

        private GameObject _gameElements;

        private CachedComponentsManager _cacheManager;
        private StateManager _stateManager;

        public void Initialize()
        {
            _gameElements = GameObject.FindGameObjectWithTag(AppConstants.GameElementsTag);
            _cacheManager = CachedComponentsManager.GetInstance();

            _stateManager = StateManager.GetInstance(_world);

            InitializeTargets();
        }

        public void Destroy()
        {
            _cacheManager.ClearTargets();
        }

        protected override EcsReactiveType GetReactiveType()
        {
            return EcsReactiveType.OnAdded;
        }

        protected override void RunReactive()
        {
            for (var i = 0; i < ReactedEntitiesCount; i++)
            {
                var entity = ReactedEntities[i];
                var target = _world.GetComponent<Target>(entity);

                CreateTarget(target);

                if (!target.Silent)
                {
                    _stateManager.AddTarget(target.Row, target.Column, target.Value, target.Round);
                }
            }
        }

        private void CreateTarget(Target element)
        {
            var target = element.GetTargetElement();
            target.gameObject.SetActive(true);
            element.Transform = target.transform;

            target.transform.position = TargetHelper.GetPositionVector(element.Row, element.Column);
            element.SetTexture();
        }

        private void InitializeTargets()
        {
            var defaultTarget = (GameObject)Resources.Load(DefaultTargetPath, typeof(GameObject));
            _cacheManager.DefaultTarget = GameObject.Instantiate(defaultTarget);
            _cacheManager.DefaultTarget.transform.parent = _gameElements.transform;
            _cacheManager.DefaultTarget.SetActive(false);

            var addTailRemoveTwoWallTarget = (GameObject)Resources.Load(AddTailRemoveTwoWallTarget, typeof(GameObject));
            _cacheManager.AddTailRemoveTwoWallTarget = GameObject.Instantiate(addTailRemoveTwoWallTarget);
            _cacheManager.AddTailRemoveTwoWallTarget.transform.parent = _gameElements.transform;
            _cacheManager.AddTailRemoveTwoWallTarget.SetActive(false);

            var removeTailAddWallTarget = (GameObject)Resources.Load(RemoveTailAddWallTarget, typeof(GameObject));
            _cacheManager.RemoveTailAddWallTarget = GameObject.Instantiate(removeTailAddWallTarget);
            _cacheManager.RemoveTailAddWallTarget.transform.parent = _gameElements.transform;
            _cacheManager.RemoveTailAddWallTarget.SetActive(false);
        }
    }
}
