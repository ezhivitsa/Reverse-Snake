using System.Collections.Generic;
using UnityEngine;

namespace Assets.ReverseSnake.Scripts.Managers
{
    public class CachedComponentsManager
    {
        private static CachedComponentsManager _instance;
        private static List<GameObject> _steps = new List<GameObject>();

        private static GameObject _addWallTarget;
        private static GameObject _removeWallTarget;
        private static GameObject _addTailRemoveTwoWallTarget;
        private static GameObject _removeTailAddWallTarget;

        public GameObject AddWallTarget {
            get
            {
                return _addWallTarget;
            }
            set
            {
                _addWallTarget = value;
            }
        }

        public GameObject RemoveWallTarget
        {
            get
            {
                return _removeWallTarget;
            }
            set
            {
                _removeWallTarget = value;
            }
        }

        public GameObject AddTailRemoveTwoWallTarget
        {
            get
            {
                return _addTailRemoveTwoWallTarget;
            }
            set
            {
                _addTailRemoveTwoWallTarget = value;
            }
        }

        public GameObject RemoveTailAddWallTarget
        {
            get
            {
                return _removeTailAddWallTarget;
            }
            set
            {
                _removeTailAddWallTarget = value;
            }
        }

        private CachedComponentsManager() {}

        public static CachedComponentsManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = new CachedComponentsManager();
            }
            return _instance;
        }

        public GameObject GetAvailableStep()
        {
            lock(_steps)
            {
                if (_steps.Count == 0)
                {
                    return null;
                }

                var step = _steps[0];
                _steps.RemoveAt(0);
                return step;
            }
        }

        public void AddAvailableStep(GameObject entity)
        {
            lock(_steps)
            {
                _steps.Add(entity);
            }
        }

        public bool HasSteps()
        {
            lock (_steps)
            {
                return _steps.Count > 0;
            }
        }

        public void Clear()
        {
            lock(_steps)
            {
                _steps.Clear();
            }
        }

        public void ClearTargets()
        {
            _addWallTarget = null;
            _removeWallTarget = null;
            _addTailRemoveTwoWallTarget = null;
            _removeTailAddWallTarget = null;
        }
    }
}
