// ----------------------------------------------------------------------------
// The MIT License
// Ui extension https://github.com/Leopotam/ecs-ui
// for ECS framework https://github.com/Leopotam/ecs
// Copyright (c) 2017-2018 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using Leopotam.Ecs.Ui.Components;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Leopotam.Ecs.Ui.Actions {
    /// <summary>
    /// Ui action for processing OnClick events.
    /// </summary>
    public sealed class EcsUiClickAction : EcsUiActionBase, IPointerClickHandler {
        [Range (1f, 2048f)]
        public float DragTreshold = 5f;

        void IPointerClickHandler.OnPointerClick (PointerEventData eventData) {
            if ((eventData.pressPosition - eventData.position).sqrMagnitude < DragTreshold * DragTreshold) {
                if ((object) Emitter != null) {
                    var msg = Emitter.CreateMessage<EcsUiClickEvent> ();
                    msg.WidgetName = WidgetName;
                    msg.Sender = gameObject;
                    msg.Position = eventData.position;
                }
            }
        }
    }
}