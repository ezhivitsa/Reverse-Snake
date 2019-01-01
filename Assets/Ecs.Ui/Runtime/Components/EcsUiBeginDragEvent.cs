// ----------------------------------------------------------------------------
// The MIT License
// Ui extension https://github.com/Leopotam/ecs-ui
// for ECS framework https://github.com/Leopotam/ecs
// Copyright (c) 2017-2018 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using UnityEngine;

namespace Leopotam.Ecs.Ui.Components {
    [EcsOneFrame]
    public sealed class EcsUiBeginDragEvent : IEcsAutoResetComponent {
        public string WidgetName;

        public GameObject Sender;

        public Vector2 Position;

        public int PointerId;

        void IEcsAutoResetComponent.Reset () {
            Sender = null;
        }
    }
}