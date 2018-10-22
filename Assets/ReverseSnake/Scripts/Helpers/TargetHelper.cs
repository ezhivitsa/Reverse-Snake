using Assets.ReverseSnake.Scripts.Enums;
using Assets.ReverseSnake.Scripts.Extensions;
using Assets.src;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.ReverseSnake.Scripts.Helpers
{
    public static class TargetHelper
    {
        private static int _firstValue = 2;
        private static int _secondValue = (AppConstants.Columns - 1) * (AppConstants.Rows - 1);
        private static int _thirdValue = (AppConstants.Columns - 1) * (AppConstants.Rows - 1) * 2;

        static public bool CanGetTargetElement(int currentNumber)
        {
            return currentNumber == 2 || currentNumber == 3;
        }

        static public TargetValueEnum GetTargetValue(int activeWalls)
        {
            if (activeWalls < _firstValue)
            {
                return TargetValueEnum.AddWall;
            }

            activeWalls = Mathf.Max(activeWalls, _thirdValue);

            var randomValue = Random.Range(0, 100);

            var addWallProbability = AddWallProbability(activeWalls);
            var removeWallProbability = RemoveWallProbability(activeWalls);
            var addTailRemoveTwoWallProbability = AddTailRemoveTwoWallProbability(activeWalls);

            if (randomValue < addWallProbability)
            {
                return TargetValueEnum.AddWall;
            }
            else if (randomValue < removeWallProbability)
            {
                return TargetValueEnum.RemoveWall;
            }
            else if (randomValue < addTailRemoveTwoWallProbability)
            {
                return TargetValueEnum.AddTailRemoveTwoWall;
            }
            else
            {
                return TargetValueEnum.RemoveTailAddWall;
            }
        }

        static private float AddWallProbability(float value)
        {
            var values = new List<TargetValueEnum> { TargetValueEnum.AddWall };
            var startProbability = GetStartProbability(values);
            var intermediateProbability = GetIntermediateProbability(values);
            var endProbability = GetEndProbability(values);

            return ProbabilityFunction(startProbability, intermediateProbability, endProbability, value);
        }

        static private float RemoveWallProbability(float value)
        {
            var values = new List<TargetValueEnum> { TargetValueEnum.AddWall, TargetValueEnum.RemoveWall };
            var startProbability = GetStartProbability(values);
            var intermediateProbability = GetIntermediateProbability(values);
            var endProbability = GetEndProbability(values);

            return ProbabilityFunction(startProbability, intermediateProbability, endProbability, value);
        }

        static private float AddTailRemoveTwoWallProbability(float value)
        {
            var values = new List<TargetValueEnum> {
                TargetValueEnum.AddWall,
                TargetValueEnum.RemoveWall,
                TargetValueEnum.AddTailRemoveTwoWall,
            };
            var startProbability = GetStartProbability(values);
            var intermediateProbability = GetIntermediateProbability(values);
            var endProbability = GetEndProbability(values);

            return ProbabilityFunction(startProbability, intermediateProbability, endProbability, value);
        }

        static private float RemoveTailAddWallProbability(float value)
        {
            var values = new List<TargetValueEnum> {
                TargetValueEnum.AddWall,
                TargetValueEnum.RemoveWall,
                TargetValueEnum.AddTailRemoveTwoWall,
                TargetValueEnum.RemoveTailAddWall,
            };
            var startProbability = GetStartProbability(values);
            var intermediateProbability = GetIntermediateProbability(values);
            var endProbability = GetEndProbability(values);

            return ProbabilityFunction(startProbability, intermediateProbability, endProbability, value);
        }

        static private float GetStartProbability(List<TargetValueEnum> values)
        {
            return values.Aggregate(0f, (acc, x) => acc + x.GetStartProbability());
        }

        static private float GetIntermediateProbability(List<TargetValueEnum> values)
        {
            return values.Aggregate(0f, (acc, x) => acc + x.GetIntermediateProbability());
        }

        static private float GetEndProbability(List<TargetValueEnum> values)
        {
            return values.Aggregate(0f, (acc, x) => acc + x.GetEndProbability());
        }

        static private float ProbabilityFunction(float start, float intermediate, float end, float value)
        {
            var calcStart = value < _secondValue ? start : intermediate;
            var calcEnd = value < _secondValue ? intermediate : end;

            var valStart = value < _secondValue ? _firstValue : _secondValue;
            var valEnd = value < _secondValue ? _secondValue : _thirdValue;

            return ((valEnd - value) * calcStart + (value + valStart) * calcEnd) / (valEnd - valStart);
        }
    }
}
