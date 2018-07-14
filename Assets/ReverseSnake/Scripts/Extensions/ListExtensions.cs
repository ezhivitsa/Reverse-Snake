using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Assets.ReverseSnake.Scripts.Extensions
{
    public static class List
    {
        public static T RandomElement<T>(this IEnumerable<T> enumerableList)
        {
            var list = enumerableList.ToList();
            var num = Random.Range(0, list.Count);
            return list[num];
        }
    }
}