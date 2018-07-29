namespace Assets.ReverseSnake.Scripts.Helpers
{
    public static class TargetHelper
    {
        static public bool CanGetTargetElement(int currentNumber)
        {
            return currentNumber == 2 || currentNumber == 3;
        }
    }
}
