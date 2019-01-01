// ----------------------------------------------------------------------------
// The MIT License
// Ui extension https://github.com/Leopotam/ecs-ui
// for ECS framework https://github.com/Leopotam/ecs
// Copyright (c) 2017-2018 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

namespace Leopotam.Ecs.Ui.Components {
    [EcsOneFrame]
    public sealed class EcsUiScrollViewEvent : IEcsAutoResetComponent {
        public string WidgetName;

        public ScrollRect Sender;

        public Vector2 Value;

        void IEcsAutoResetComponent.Reset () {
            Sender = null;
        }
    }
}