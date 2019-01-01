using System.Collections.Generic;
using UnityEngine;

namespace Assets.ReverseSnake.Scripts.Managers
{
    public class DisabledStepsManager
    {
        private static DisabledStepsManager _instance;
        private static List<GameObject> _steps = new List<GameObject>();

        private DisabledStepsManager() {}

        public static DisabledStepsManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = new DisabledStepsManager();
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
    }
}
