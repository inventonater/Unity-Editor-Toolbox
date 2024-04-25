using System;
using UnityEditor;
using System.Runtime.InteropServices;
using UnityEditorInternal;
using UnityEngine;

namespace Toolbox.Editor
{
    /// <summary>
    /// Provides mouse and keyboard navigation functionality in the Unity Editor.
    /// </summary>
    [InitializeOnLoad]
    public static class SelectionHistoryNavigationShortcuts
    {
        public static event Action ForwardButtonDown = delegate { };
        public static event Action BackwardButtonDown = delegate { };

        private const int MenuItemPriority = 100;
        private const string MenuItemRoot = "Window/Selection History/";

        [MenuItem(MenuItemRoot + "Open", priority = MenuItemPriority)]
        public static void ShowWindow() {
            SelectionHistoryWindow selectionHistoryWindow = EditorWindow.GetWindow<SelectionHistoryWindow>();
            selectionHistoryWindow.titleContent = new GUIContent("Selection History");
            selectionHistoryWindow.RefreshElements(); // Do refresh the list to highlight the new selection.
        }

        /// <summary>
        /// Navigates forward in the selection history with ] shortcut
        /// </summary>
        [MenuItem(MenuItemRoot + "Forward _]", false, MenuItemPriority + 1)]
        private static void NavigateForward() => ForwardButtonDown();

        /// <summary>
        /// Navigates backward in the selection history with [ shortcut.
        /// </summary>
        [MenuItem(MenuItemRoot + "Back _[", false, MenuItemPriority + 2)]
        private static void NavigateBackward() => BackwardButtonDown();

        /// <summary>
        /// Static constructor to initialize the ToolboxEditorMouseNavigation.
        /// </summary>
        static SelectionHistoryNavigationShortcuts()
        {
            EditorApplication.update += PollMouseButtons;
        }

        private static bool _wasForwardPressed;
        private static bool _wasBackwardPressed;

        private static void PollMouseButtons()
        {
            if (!InternalEditorUtility.isApplicationActive) return;

            bool isForwardPressed = IsForwardPressed;
            bool isBackwardPressed = IsBackwardPressed;

            if (isForwardPressed && !_wasForwardPressed) ForwardButtonDown();
            if (isBackwardPressed && !_wasBackwardPressed) BackwardButtonDown();

            _wasForwardPressed = isForwardPressed;
            _wasBackwardPressed = isBackwardPressed;
        }

#if UNITY_EDITOR_WIN
        /// <summary>
        /// Windows only, we can get fetch the Forward/Back buttons on the Mouse via GetAsyncKeyState
        /// </summary>
        /// <param name="virtualKeyCode"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(ushort virtualKeyCode);
        private const ushort ForwardVirtualKeyCode = 0x06;
        private const ushort BackwardVirtualKeyCode = 0x05;

        private static bool IsForwardPressed => GetAsyncKeyState(ForwardVirtualKeyCode) != 0;
        private static bool IsBackwardPressed => GetAsyncKeyState(BackwardVirtualKeyCode) != 0;
#else
        private static bool IsBackwardPressed => false;
        private static bool IsForwardPressed => false;
#endif

    }
}
