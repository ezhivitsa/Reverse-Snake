using Leopotam.Ecs;
using System;
using System.Collections.Generic;

namespace Assets.ReverseSnake.Scripts.Extensions
{
    public static class EcsFilterExtensions
    {
        public static List<T> ToEntitiesList<T>(this EcsFilter<T> filter) where T : class, new()
        {
            var result = new List<T>();
            foreach (var idx in filter)
            {
                result.Add(filter.Components1[idx]);
            }

            return result;
        }

        public static List<EcsEntity> ToEntitieNumbersList<T>(this EcsFilter<T> filter) where T : class, new()
        {
            var result = new List<EcsEntity>();
            foreach (var idx in filter)
            {
                result.Add(filter.Entities[idx]);
            }

            return result;
        }

        public static void HandleEvents<T>(this EcsFilter<T> filter, EcsWorld _world, Action<T> action) where T : class, new()
        {
            foreach (var idx in filter)
            {
                action(filter.Components1[idx]);
                _world.RemoveEntity(filter.Entities[idx]);
            }
        }
    }
}
