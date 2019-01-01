// ----------------------------------------------------------------------------
// The MIT License
// Reactive behaviour for Entity Component System framework https://github.com/Leopotam/ecs
// Copyright (c) 2017-2018 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

namespace Leopotam.Ecs {
    public static class EcsWorldExtensions {
        /// <summary>
        /// Marks component on entity as updated for processing later at reactive system.
        /// </summary>
        /// <typeparam name="T">Component type.</typeparam>
        public static void MarkComponentAsUpdated<T> (this EcsWorld world, int entity) where T : class, new () {
            bool isNew;
            world.EnsureComponent<Reactive.EcsUpdateReactiveFlag<T>> (entity, out isNew);
        }
    }
}