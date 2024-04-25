using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Toolbox.Editor
{
    [InitializeOnLoad]
    [FilePath("Library/history.asset", FilePathAttribute.Location.ProjectFolder)]
    public class SelectionHistoryData : ScriptableSingleton<SelectionHistoryData>
    {
        private static Object _objectToSkip;
        private static double _skipNextTime;
        public static void SkipNextSelectionChangedHistory(Object objectToSkip)
        {
            _objectToSkip = objectToSkip;
            _skipNextTime = EditorApplication.timeSinceStartup;
        }

        static SelectionHistoryData()
        {
            // Register these callbacks on editor load.
            Selection.selectionChanged += OnSelectionChanged;
            EditorSceneManager.sceneClosing += OnSceneClosing;
            EditorApplication.quitting += OnEditorQuitting;
            SelectionHistoryNavigationShortcuts.ForwardButtonDown += () => instance.NavigateHistory(1);
            SelectionHistoryNavigationShortcuts.BackwardButtonDown += () => instance.NavigateHistory(-1);
        }

        private void NavigateHistory(int steps)
        {
            if (history.Count == 0) return;

            _navigationIndex = Mathf.Clamp(_navigationIndex + steps, 0, history.Count - 1);
            var activeObject = history[_navigationIndex];
            SkipNextSelectionChangedHistory(activeObject);
            Selection.activeObject = activeObject;

            if (SelectionHistoryPreferences.AutoShowHistoryWindow) SelectionHistoryNavigationShortcuts.ShowWindow();
        }

        // Public Properties
        public event Action OnChanged = delegate { };
        public Object lastNonPinnedElement => history.Count == 0 ? null : history[history.Count - 1];

        // Private State
        [SerializeField] private List<Object> history = new List<Object>();
        public List<Object> History => history;

        private int _navigationIndex;
        public int NavigationIndex => _navigationIndex;

        private void OnEnable()
        {
            Clear();
        }

        public void Clear()
        {
            _navigationIndex = 0;
            history.Clear();
        }

        private void NotifyDataChanged()
        {
            history.RemoveAll(o => o == null);
            SetNavigationIndexFromSelectedObject();
            OnChanged?.Invoke();
        }

        public void Add(Object activeObject)
        {
            if (activeObject == null) return;
            if (_navigationIndex < history.Count - 1) history.RemoveRange(_navigationIndex + 1, history.Count - _navigationIndex - 1);
            history.Add(activeObject);
            while (history.Count > SelectionHistoryPreferences.HistoryMax) history.RemoveAt(0);
            NotifyDataChanged();
        }

        #region Static Callbacks

        private static void OnEditorQuitting()
        {
            instance.Save(true);
        }

        private static void OnSelectionChanged()
        {
            var activeObject = Selection.activeObject;
            if (_objectToSkip == activeObject && EditorApplication.timeSinceStartup - _skipNextTime < 0.2f)
            {
                // Avoid recording new history from history navigation actions
                _objectToSkip = null;
                return;
            }

            instance.Add(activeObject);
        }

        private static void OnSceneClosing(UnityEngine.SceneManagement.Scene scene, bool removingScene)
        {
            // Throws the last scene into the history, since I often want to go back anyway.
            var asset = AssetDatabase.LoadAssetAtPath<Object>(scene.path);
            instance.Add(asset);
        }

        #endregion

        public void SelectObjectFromHistory(Object obj)
        {
            SkipNextSelectionChangedHistory(obj);
            Selection.activeObject = obj;
            SetNavigationIndexFromSelectedObject();
            SelectionHistoryNavigationShortcuts.ShowWindow(); // Do refresh the list to highlight the new selection.
        }

        private void SetNavigationIndexFromSelectedObject()
        {
            _navigationIndex = history.Count - 1;
            for (int i = 0; i < history.Count; i++)
            {
                if (history[i] == Selection.activeObject)
                {
                    _navigationIndex = i;
                }
            }

        }
    }
}
