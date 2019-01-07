using Assets.ReverseSnake.Scripts.Enums;
using Assets.ReverseSnake.Scripts.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.ReverseSnake.Scripts.Extensions
{
    static class TargetExtensions
    {
        const string MaterialsPath = "Materials/Textures";

        public static void SetTexture(this Target element)
        {
            if (element.Value == TargetValueEnum.AddWall || element.Value == TargetValueEnum.RemoveWall)
            {
                var textElement = element.Transform.GetChild(0).GetComponent<TextMesh>();
                textElement.text = element.Value.GetDescription();
                textElement.color = GetTargetColor(element.Value);

                var renderer = element.Transform.GetChild(1).GetComponent<Renderer>();
                var name = element.Value.GetTextureName();
                var material = (Material)Resources.Load($"{MaterialsPath}/{name}", typeof(Material));
                renderer.material = material;
            }
        }

        static public GameObject GetTargetElement(this Target target)
        {
            var cacheManager = CachedComponentsManager.GetInstance();

            switch (target.Value)
            {
                case TargetValueEnum.AddTailRemoveTwoWall:
                    return cacheManager.AddTailRemoveTwoWallTarget;

                case TargetValueEnum.RemoveTailAddWall:
                    return cacheManager.RemoveTailAddWallTarget;

                default:
                    return cacheManager.DefaultTarget;
            }
        }

        static private Color GetTargetColor(TargetValueEnum value)
        {
            switch (value)
            {
                case TargetValueEnum.AddWall:
                    return new Color32(51, 135, 77, 255);

                case TargetValueEnum.RemoveWall:
                    return new Color32(220, 85, 98, 225);

                default:
                    return Color.white;
            }
        }
    }
}
