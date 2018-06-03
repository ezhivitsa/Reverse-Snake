using System.Collections.Generic;

namespace Assets.src.Managers
{
    public class StepsManager
    {
        private int _availableSteps;
        private int _leftSteps;

        private List<int> _positions;
        private int _lastPosition;

        public bool HasSteps
        {
            get
            {
                return _leftSteps >= 1;
            }
        }

        public bool CanGetTarget
        {
            get
            {
                return _leftSteps == 1 || _leftSteps == 2;
            }
        }

        public StepsManager(int availableSteps)
        {
            _availableSteps = availableSteps;
            _leftSteps = availableSteps;

            _positions = new List<int>();
            _lastPosition = -1;
        }

        public int GetLeftSteps()
        {
            return _leftSteps;
        }

        public void Add(int position)
        {
            _leftSteps -= 1;
            _positions.Add(position);
            _lastPosition = position;
        }

        public int LastPosition()
        {
            return _lastPosition;
        }

        public List<int> Reset()
        {
            var oldPostions = _positions;

            _availableSteps = _availableSteps + 1;
            _leftSteps = _availableSteps;

            _positions = new List<int>();
            _lastPosition = -1;

            return oldPostions;
        }

        public void IncreaseAvailableSteps(int value)
        {
            _availableSteps += value;
        }
    }
}
