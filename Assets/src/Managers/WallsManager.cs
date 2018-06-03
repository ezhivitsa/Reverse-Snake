namespace Assets.src.Managers
{
    public class WallsManager
    {
        private int _addNewOnStep;
        private int _currentStep;

        public bool ShouldAddWall
        {
            get
            {
                return _currentStep >= _addNewOnStep;
            }
        }

        public WallsManager(int addNewOnStep)
        {
            _addNewOnStep = addNewOnStep;
            _currentStep = 0;
        }

        public void DoStep()
        {
            _currentStep += 1;
        }

        public void AddWall()
        {
            _currentStep = 0;
        }
    }
}
