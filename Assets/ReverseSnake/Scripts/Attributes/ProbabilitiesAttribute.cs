using System;

namespace Assets.ReverseSnake.Scripts.Attributes
{
    class ProbabilitiesAttribute : Attribute
    {
        internal ProbabilitiesAttribute(float startProbability, float endProbability)
        {
            StartProbability = startProbability;
            EndProbability = endProbability;
        }
        public float StartProbability { get; private set; }
        public float EndProbability { get; private set; }
    }
}
