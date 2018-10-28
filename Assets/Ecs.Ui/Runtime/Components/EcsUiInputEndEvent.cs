// ----------------------------------------------------------------------------
// The MIT License
// Ui extension https://github.com/Leopotam/ecs-ui
// for ECS framework https://github.com/Leopotam/ecs
// Copyright (c) 2018 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using UnityEngine.UI;

namespace Leopotam.Ecs.Ui.Components {
    public sealed class EcsUiInputEndEvent {
        public string WidgetName;

        public InputField Sender;

        public string Value;
    }
}