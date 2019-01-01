// ----------------------------------------------------------------------------
// The MIT License
// Ui extension https://github.com/Leopotam/ecs-ui
// for ECS framework https://github.com/Leopotam/ecs
// Copyright (c) 2017-2018 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using Leopotam.Ecs.Ui.Components;
using UnityEngine;
using UnityEngine.UI;

namespace Leopotam.Ecs.Ui.Actions {
    /// <summary>
    /// Ui action for processing InputField events.
    /// </summary>
    [RequireComponent (typeof (InputField))]
    public sealed class EcsUiInputAction : EcsUiActionBase {
        InputField _input;

        void Awake () {
            _input = GetComponent<InputField> ();
            _input.onValueChanged.AddListener (OnInputValueChanged);
            _input.onEndEdit.AddListener (OnInputEnded);
        }

        void OnInputValueChanged (string value) {
            if ((object) Emitter != null) {
                var msg = Emitter.CreateMessage<EcsUiInputChangeEvent> ();
                msg.WidgetName = WidgetName;
                msg.Sender = _input;
                msg.Value = value;
            }
        }

        void OnInputEnded (string value) {
            if ((object) Emitter != null) {
                var msg = Emitter.CreateMessage<EcsUiInputEndEvent> ();
                msg.WidgetName = WidgetName;
                msg.Sender = _input;
                msg.Value = value;
            }
        }
    }
}