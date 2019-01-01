// ----------------------------------------------------------------------------
// The MIT License
// Ui extension https://github.com/Leopotam/ecs-ui
// for ECS framework https://github.com/Leopotam/ecs
// Copyright (c) 2017-2018 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using Leopotam.Ecs.Ui.Components;
using UnityEngine.EventSystems;

namespace Leopotam.Ecs.Ui.Actions {
    /// <summary>
    /// Ui action for processing OnDrop events.
    /// </summary>
    public sealed class EcsUiDropAction : EcsUiActionBase, IDropHandler {
        void IDropHandler.OnDrop (PointerEventData eventData) {
            if ((object) Emitter != null) {
                var msg = Emitter.CreateMessage<EcsUiDropEvent> ();
                msg.WidgetName = WidgetName;
                msg.Sender = gameObject;
            }
        }
    }
}