// ----------------------------------------------------------------------------
// The MIT License
// Ui extension https://github.com/Leopotam/ecs-ui
// for ECS framework https://github.com/Leopotam/ecs
// Copyright (c) 2018 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using Leopotam.Ecs.Ui.Components;

namespace Leopotam.Ecs.Ui.Systems {
#if !LEOECS_DISABLE_INJECT
    [EcsInject]
#endif
    public class EcsUiCleaner : IEcsRunSystem {
        EcsWorld _world = null;

        EcsFilter<EcsUiClickEvent> _clickEvents = null;

        EcsFilter<EcsUiBeginDragEvent> _beginDragEvents = null;

        EcsFilter<EcsUiDragEvent> _dragEvents = null;

        EcsFilter<EcsUiEndDragEvent> _endDragEvents = null;

        EcsFilter<EcsUiEnterEvent> _enterEvents = null;

        EcsFilter<EcsUiExitEvent> _exitEvents = null;

        EcsFilter<EcsUiInputChangeEvent> _inputChangeEvents = null;

        EcsFilter<EcsUiInputEndEvent> _inputEndEvents = null;

        EcsFilter<EcsUiScrollViewEvent> _scrollViewEvents = null;

#if LEOECS_DISABLE_INJECT
        /// <summary>
        /// Sets EcsWorld instance.
        /// </summary>
        /// <param name="world">Instance.</param>
        public EcsUiCleaner SetWorld (EcsWorld world) {
            _world = world;
            ValidateEcsFields ();
            _clickEvents = _world.GetFilter<EcsFilter<EcsUiClickEvent>> ();
            _beginDragEvents = _world.GetFilter<EcsFilter<EcsUiBeginDragEvent>> ();
            _dragEvents = _world.GetFilter<EcsFilter<EcsUiDragEvent>> ();
            _endDragEvents = _world.GetFilter<EcsFilter<EcsUiEndDragEvent>> ();
            _enterEvents = _world.GetFilter<EcsFilter<EcsUiEnterEvent>> ();
            _exitEvents = _world.GetFilter<EcsFilter<EcsUiExitEvent>> ();
            _inputChangeEvents = _world.GetFilter<EcsFilter<EcsUiInputChangeEvent>> ();
            _inputEndEvents = _world.GetFilter<EcsFilter<EcsUiInputEndEvent>> ();
            _scrollViewEvents = _world.GetFilter<EcsFilter<EcsUiScrollViewEvent>> ();
            return this;
        }
#endif

        void IEcsRunSystem.Run () {
            ValidateEcsFields ();
            foreach (var i in _clickEvents) {
                _clickEvents.Components1[i].Sender = null;
                _world.RemoveEntity (_clickEvents.Entities[i]);
            }
            foreach (var i in _beginDragEvents) {
                _beginDragEvents.Components1[i].Sender = null;
                _world.RemoveEntity (_beginDragEvents.Entities[i]);
            }
            foreach (var i in _dragEvents) {
                _dragEvents.Components1[i].Sender = null;
                _world.RemoveEntity (_dragEvents.Entities[i]);
            }
            foreach (var i in _endDragEvents) {
                _endDragEvents.Components1[i].Sender = null;
                _world.RemoveEntity (_endDragEvents.Entities[i]);
            }
            foreach (var i in _enterEvents) {
                _enterEvents.Components1[i].Sender = null;
                _world.RemoveEntity (_enterEvents.Entities[i]);
            }
            foreach (var i in _exitEvents) {
                _exitEvents.Components1[i].Sender = null;
                _world.RemoveEntity (_exitEvents.Entities[i]);
            }
            foreach (var i in _inputChangeEvents) {
                _inputChangeEvents.Components1[i].Sender = null;
                _world.RemoveEntity (_inputChangeEvents.Entities[i]);
            }
            foreach (var i in _inputEndEvents) {
                _inputEndEvents.Components1[i].Sender = null;
                _world.RemoveEntity (_inputEndEvents.Entities[i]);
            }
            foreach (var i in _scrollViewEvents) {
                _scrollViewEvents.Components1[i].Sender = null;
                _world.RemoveEntity (_scrollViewEvents.Entities[i]);
            }
        }

        [System.Diagnostics.Conditional ("DEBUG")]
        void ValidateEcsFields () {
            if (_world == null) {
                throw new System.Exception ("[EcsUiCleaner] Call SetWorld() method first with valid world instance.");
            }
        }
    }
}