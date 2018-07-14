using LeopotamGroup.Ecs;
using System.Collections.Generic;

namespace Assets.ReverseSnake.Scripts.Extensions
{
    public static class EcsFilterExtensions
    {
        public static List<T> ToEntitiesList<T>(this EcsFilter<T> filter) where T : class, new()
        {
            var result = new List<T>();
            for (var i = 0; i < filter.EntitiesCount; i++)
            {
                result.Add(filter.Components1[i]);
            }

            return result;
        }
    }
}
