using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace Toolbox.Editor
{
    /// <summary>
    /// Provides mouse and keyboard navigation functionality in the Unity Editor.
    /// </summary>
    [InitializeOnLoad]
    public static class ToolboxEditorSelectionNavigation
    {
        private const string BackwardHistoryKey = "ToolboxEditorSelectionNavigation.BackwardHistory";
        private const string ForwardHistoryKey = "ToolboxEditorSelectionNavigation.ForwardHistory";

        private const int MouseButtonBack = 3; // 'back' button for Event.current events
        private const int MouseButtonForward = 4; // 'forward' button for Event.current events
        public static KeyCode BackHotkey = KeyCode.LeftBracket;
        public static KeyCode ForwardHotkey = KeyCode.RightBracket;

        private static Stack<int> _backwardHistory = new();
        private static Stack<int> _forwardHistory = new();

        private static bool _skipNextSelectionChanged;

        /// <summary>
        /// Static constructor to initialize the ToolboxEditorMouseNavigation.
        /// </summary>
        static ToolboxEditorSelectionNavigation()
        {
            LoadHistory();
            SubscribeEvents();
        }

        private static void SubscribeEvents()
        {
            Selection.selectionChanged -= SelectionChanged;
            Selection.selectionChanged += SelectionChanged;

            EditorApplication.playModeStateChanged -= PlayModeStateChanged;
            EditorApplication.playModeStateChanged += PlayModeStateChanged;

            AssemblyReloadEvents.beforeAssemblyReload -= SaveHistory;
            AssemblyReloadEvents.beforeAssemblyReload += SaveHistory;
            AssemblyReloadEvents.afterAssemblyReload -= LoadHistory;
            AssemblyReloadEvents.afterAssemblyReload += LoadHistory;

            UnityEditor.SceneView.duringSceneGui -= SceneViewDuringSceneGui;
            UnityEditor.SceneView.duringSceneGui += SceneViewDuringSceneGui;
            EditorApplication.hierarchyWindowItemOnGUI -= HierarchyWindowItemOnGui;
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGui;
            EditorApplication.projectWindowItemOnGUI -= ProjectWindowItemOnGui;
            EditorApplication.projectWindowItemOnGUI += ProjectWindowItemOnGui;
        }

        private static void HierarchyWindowItemOnGui(int instanceId, Rect selectionRect) => HandleMouseEvents();
        private static void SceneViewDuringSceneGui(UnityEditor.SceneView sceneView) => HandleMouseEvents();
        private static void ProjectWindowItemOnGui(string guid, Rect selectionRect) => HandleMouseEvents();

        /// <summary>
        /// Called when the selection in the Unity Editor changes.
        /// </summary>
        private static void SelectionChanged()
        {
            if (_skipNextSelectionChanged || Selection.activeObject == null)
            {
                _skipNextSelectionChanged = false;
                return;
            }

            _backwardHistory.Push(Selection.activeInstanceID);
            _forwardHistory.Clear();
        }

        /// <summary>
        /// Handles the mouse navigation events in the Unity Editor for valid window focus
        /// </summary>
        private static void HandleMouseEvents()
        {
            Event e = Event.current;
            if (e is { type: EventType.MouseDown, button: MouseButtonBack })
            {
                NavigateBackward();
                e.Use();
            }

            if (e is { type: EventType.MouseDown, button: MouseButtonForward })
            {
                NavigateForward();
                e.Use();
            }
        }

        /// <summary>
        /// Navigates backward in the selection history.
        /// </summary>
        [MenuItem("Tools/Navigation/Back _[", false, 100)]
        private static void NavigateBackward()
        {
            if (_backwardHistory.Count <= 1) return;
            _forwardHistory.Push(_backwardHistory.Pop());
            Selection.activeInstanceID = _backwardHistory.Peek();
            _skipNextSelectionChanged = true;
        }

        /// <summary>
        /// Navigates forward in the selection history.
        /// </summary>
        [MenuItem("Tools/Navigation/Forward _]", false, 101)]
        private static void NavigateForward()
        {
            if (_forwardHistory.Count <= 0) return;
            _backwardHistory.Push(_forwardHistory.Pop());
            Selection.activeInstanceID = _backwardHistory.Peek();
            _skipNextSelectionChanged = true;
        }

        /// <summary>
        /// Clears the navigation history.
        /// </summary>
        [MenuItem("Tools/Navigation/Clear History", false, 102)]
        private static void ClearHistory()
        {
            _backwardHistory.Clear();
            _forwardHistory.Clear();
            SaveHistory();
            Debug.Log("Navigation history cleared.");
        }

        private static void PlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode) SaveHistory();
            if (state == PlayModeStateChange.EnteredEditMode) LoadHistory();
        }

        /// <summary>
        /// Loads the navigation history from SessionState.
        /// </summary>
        private static void LoadHistory()
        {
            _backwardHistory = new Stack<int>(SessionState.GetIntArray(BackwardHistoryKey, Array.Empty<int>()));
            _forwardHistory = new Stack<int>(SessionState.GetIntArray(ForwardHistoryKey, Array.Empty<int>()));
        }

        /// <summary>
        /// Saves the navigation history to SessionState.
        /// </summary>
        private static void SaveHistory()
        {
            SessionState.SetIntArray(BackwardHistoryKey, _backwardHistory.ToArray());
            SessionState.SetIntArray(ForwardHistoryKey, _forwardHistory.ToArray());
        }
    }
}