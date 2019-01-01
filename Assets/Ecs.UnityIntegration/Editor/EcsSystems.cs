// ----------------------------------------------------------------------------
// The MIT License
// Unity integration https://github.com/Leopotam/ecs-unityintegration
// for ECS framework https://github.com/Leopotam/ecs
// Copyright (c) 2017-2018 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Leopotam.Ecs.UnityIntegration.Editor {
    [CustomEditor (typeof (EcsSystemsObserver))]
    sealed class EcsSystemsObserverInspector : UnityEditor.Editor {
        static IEcsPreInitSystem[] _preInitList = new IEcsPreInitSystem[32];
        static Stack<IEcsInitSystem[]> _initList = new Stack<IEcsInitSystem[]> (8);
        static Stack<IEcsRunSystem[]> _runList = new Stack<IEcsRunSystem[]> (8);

        public override void OnInspectorGUI () {
            var savedState = GUI.enabled;
            GUI.enabled = true;
            var observer = target as EcsSystemsObserver;
            var systems = observer.GetSystems ();
            int count;

            count = systems.GetPreInitSystems (ref _preInitList);
            if (count > 0) {
                GUILayout.BeginVertical (GUI.skin.box);
                EditorGUILayout.LabelField ("PreInitialize systems", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                for (var i = 0; i < count; i++) {
                    EditorGUILayout.LabelField (_preInitList[i].GetType ().Name);
                    _preInitList[i] = null;
                }
                EditorGUI.indentLevel--;
                GUILayout.EndVertical ();
            }

            GUILayout.BeginVertical (GUI.skin.box);
            EditorGUILayout.LabelField ("Initialize systems", EditorStyles.boldLabel);
            OnInitSystemsGUI (systems);
            GUILayout.EndVertical ();

            GUILayout.BeginVertical (GUI.skin.box);
            EditorGUILayout.LabelField ("Run systems", EditorStyles.boldLabel);
            OnRunSystemsGUI (systems);
            GUILayout.EndVertical ();

            GUI.enabled = savedState;
        }

        void OnInitSystemsGUI (EcsSystems systems) {
            var initList = _initList.Count > 0 ? _initList.Pop () : null;
            var count = systems.GetInitSystems (ref initList);
            if (count > 0) {
                EditorGUI.indentLevel++;
                for (var i = 0; i < count; i++) {
                    var asSystems = initList[i] as EcsSystems;
                    EditorGUILayout.LabelField (asSystems != null ? asSystems.Name : initList[i].GetType ().Name);
                    if (asSystems != null) {
                        OnInitSystemsGUI (asSystems);
                    }
                    initList[i] = null;
                }
                EditorGUI.indentLevel--;
            }
            _initList.Push (initList);
        }

        void OnRunSystemsGUI (EcsSystems systems) {
            var runList = _runList.Count > 0 ? _runList.Pop () : null;
            var count = systems.GetRunSystems (ref runList);
            if (count > 0) {
                EditorGUI.indentLevel++;
                for (var i = 0; i < count; i++) {
                    var asSystems = runList[i] as EcsSystems;
                    var name = asSystems != null ? asSystems.Name : runList[i].GetType ().Name;
                    systems.DisabledInDebugSystems[i] = !EditorGUILayout.ToggleLeft (name, !systems.DisabledInDebugSystems[i]);
                    if (asSystems != null) {
                        GUI.enabled = !systems.DisabledInDebugSystems[i];
                        OnRunSystemsGUI (asSystems);
                        if (systems.DisabledInDebugSystems[i]) {
                            GUI.enabled = true;
                        }
                    }
                    runList[i] = null;
                }
                EditorGUI.indentLevel--;
            }
            _runList.Push (runList);
        }
    }
}