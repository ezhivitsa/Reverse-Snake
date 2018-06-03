using System.Collections.Generic;

namespace Assets.src.Managers
{
    public class TargetsManager
    {
        private List<int> _positions;

        public TargetsManager()
        {
            _positions = new List<int>();
        }

        public void Add(int position)
        {
            _positions.Add(position);
        }

        public List<int> Reset()
        {
            var oldPostions = _positions;

            _positions = new List<int>();

            return oldPostions;
        }
    }
}
