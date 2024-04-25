using System;
using UnityEditor;

namespace Toolbox.Editor
{
    public static class WindowUtility
    {
        private static readonly Type SceneHierarchyWindowType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
        private static readonly Type InspectorWindowType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.InspectorWindow");

        public static EditorWindow GetSceneHierarchyWindow() => EditorWindow.GetWindow(SceneHierarchyWindowType);
        public static EditorWindow GetInspectorWindow() => EditorWindow.GetWindow(InspectorWindowType);
    }
}
