using System;

namespace Assets.ReverseSnake.Scripts.Attributes
{
    class ProbabilitiesAttribute : Attribute
    {
        internal ProbabilitiesAttribute(
            float startProbability,
            float intermediateProbability,
            float endProbability
        )
        {
            StartProbability = startProbability;
            IntermediateProbability = intermediateProbability;
            EndProbability = endProbability;
        }

        public float StartProbability { get; private set; }
        public float IntermediateProbability { get; private set; }
        public float EndProbability { get; private set; }
    }
}
