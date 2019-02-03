using Assets.ReverseSnake.Scripts.Enums;
using Assets.ReverseSnake.Scripts.Managers;
using UnityEngine;

namespace Assets.ReverseSnake.Scripts.Extensions
{
    static class TargetExtensions
    {
        static public GameObject GetTargetElement(this Target target)
        {
            var cacheManager = CachedComponentsManager.GetInstance();

            switch (target.Value)
            {
                case TargetValueEnum.AddWall:
                    return cacheManager.AddWallTarget;

                case TargetValueEnum.RemoveWall:
                    return cacheManager.RemoveWallTarget;

                case TargetValueEnum.AddTailRemoveTwoWall:
                    return cacheManager.AddTailRemoveTwoWallTarget;

                default:
                    return cacheManager.RemoveTailAddWallTarget;
            }
        }
    }
}
