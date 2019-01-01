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
    /// Ui action for processing enter / exit cursor events.
    /// </summary>
    public sealed class EcsUiEnterExitAction : EcsUiActionBase, IPointerEnterHandler, IPointerExitHandler {
        void IPointerEnterHandler.OnPointerEnter (PointerEventData eventData) {
            if ((object) Emitter != null) {
                var msg = Emitter.CreateMessage<EcsUiEnterEvent> ();
                msg.WidgetName = WidgetName;
                msg.Sender = gameObject;
            }
        }

        void IPointerExitHandler.OnPointerExit (PointerEventData eventData) {
            if ((object) Emitter != null) {
                var msg = Emitter.CreateMessage<EcsUiExitEvent> ();
                msg.WidgetName = WidgetName;
                msg.Sender = gameObject;
            }
        }
    }
}