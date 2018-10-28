// ----------------------------------------------------------------------------
// The MIT License
// Ui extension https://github.com/Leopotam/ecs-ui
// for ECS framework https://github.com/Leopotam/ecs
// Copyright (c) 2018 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using Leopotam.Ecs.Ui.Systems;
using UnityEngine;

namespace Leopotam.Ecs.Ui.Actions {
    enum EcsUiActionNameRegistrationType {
        None,
        OnAwake,
        OnStart
    }
    /// <summary>
    /// Base class for ui action.
    /// </summary>
    public abstract class EcsUiActionBase : MonoBehaviour {
        /// <summary>
        /// Logical name for filtering widgets.
        /// </summary>
        [SerializeField]
        protected string WidgetName;

        /// <summary>
        /// Ecs entities emitter.
        /// </summary>
        [SerializeField]
        protected EcsUiEmitter Emitter;

        [SerializeField]
        EcsUiActionNameRegistrationType _nameRegistrationType = EcsUiActionNameRegistrationType.None;

        void Awake () {
            if (_nameRegistrationType == EcsUiActionNameRegistrationType.OnAwake) {
                ValidateEmitter ();
                RegisterName (true);
            }
        }

        void Start () {
            ValidateEmitter ();
            if (_nameRegistrationType == EcsUiActionNameRegistrationType.OnStart) {
                RegisterName (true);
            }
        }

        void OnDestroy () {
            RegisterName (false);
        }

        void ValidateEmitter () {
            if ((object) Emitter == null) {
                Emitter = GetComponentInParent<EcsUiEmitter> ();
            }
#if DEBUG && !ECS_PERF_TEST
            if ((object) Emitter == null) {
                Debug.LogError ("EcsUiEmitter not found in hierarchy", this);
            }
#endif
        }

        void RegisterName (bool state) {
            // can be destroyed before, cant use fast null check.
            if (Emitter) {
                Emitter.SetNamedObject (WidgetName, state ? gameObject : null);
            }
        }

        /// <summary>
        /// Helper to add ecs actions from code.
        /// </summary>
        /// <param name="go">GameObject holder.</param>
        /// <param name="widgetName">Optional logical widget name, will be registered with OnStart type.</param>
        /// <param name="emitter">Optional emitter. If not provided - will be detected automatically.</param>
        public static T AddAction<T> (GameObject go, string widgetName = null, EcsUiEmitter emitter = null) where T : EcsUiActionBase {
            var action = go.AddComponent<T> ();
            if (widgetName != null) {
                action.WidgetName = widgetName;
                action._nameRegistrationType = EcsUiActionNameRegistrationType.OnStart;
            }
            action.Emitter = emitter;
            return action;
        }
    }
}