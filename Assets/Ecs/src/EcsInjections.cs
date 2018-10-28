// ----------------------------------------------------------------------------
// The MIT License
// Simple Entity Component System framework https://github.com/Leopotam/ecs
// Copyright (c) 2017-2018 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------
#if !LEOECS_DISABLE_INJECT
using System;
using System.Reflection;

namespace Leopotam.Ecs {
    /// <summary>
    /// Attribute for automatic DI injection at ECS systems.
    /// </summary>
    [AttributeUsage (AttributeTargets.Class)]
    public sealed class EcsInjectAttribute : Attribute { }

    /// <summary>
    /// Processes dependency injection to ecs systems.
    /// </summary>
#if ENABLE_IL2CPP
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption (Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption (Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
#endif
    public static class EcsInjections {
        /// <summary>
        /// Injects EcsWorld / EcsFilter fields to IEcsSystem.
        /// </summary>
        /// <param name="system">System to scan for injection.</param>
        /// <param name="world">EcsWorld instance to inject.</param>
        public static void Inject (IEcsSystem system, EcsWorld world, System.Collections.Generic.Dictionary<Type, object> injections) {
            var systemType = system.GetType ();
            if (!Attribute.IsDefined (systemType, typeof (EcsInjectAttribute))) {
                return;
            }
            var worldType = world.GetType ();
            var filterType = typeof (EcsFilter);

            foreach (var f in systemType.GetFields (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)) {
                // EcsWorld
                if (f.FieldType.IsAssignableFrom (worldType) && !f.IsStatic) {
                    f.SetValue (system, world);
                    continue;
                }
                // EcsFilter
                Internals.EcsHelpers.Assert (f.FieldType != filterType,
                    () => string.Format ("Cant use EcsFilter type at \"{0}\" system for dependency injection, use generic version instead", system));
                if (f.FieldType.IsSubclassOf (filterType) && !f.IsStatic) {
                    f.SetValue (system, world.GetFilter (f.FieldType));
                    continue;
                }
                // Other injections.
                foreach (var pair in injections) {
                    if (f.FieldType.IsAssignableFrom (pair.Key) && !f.IsStatic) {
                        f.SetValue (system, pair.Value);
                        break;
                    }
                }
            }
        }
    }
}
#endif