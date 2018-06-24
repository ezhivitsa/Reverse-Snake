using UnityEngine;
using System.Collections.Generic;

namespace Assets.src.Helpers
{
    public static class ListExtensions
    {
        public static T RandomElement<T>(this List<T> list)
        {
            var num = Random.Range(0, list.Count);
            return list[num];
        }
    }
}
