using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Toolbox.Editor
{
    public class SelectionHistoryWindow : EditorWindow, IHasCustomMenu
    {
        // The template each element in the history window.
        public VisualTreeAsset windowTemplate;
        public VisualTreeAsset elementTemplate;

        private ScrollView scrollView;
        private List<VisualElement> elements = new List<VisualElement>();
        private static bool _isVisible;
        public static bool IsVisible => _isVisible;

        private void OnBecameVisible() => _isVisible = true;

        private void OnBecameInvisible() => _isVisible = false;

        private void OnFocus()
        {
            SetClass(rootVisualElement, "LightSkin", !EditorGUIUtility.isProSkin);
            rootVisualElement.AddToClassList("Focused");
        }

        private void OnLostFocus()
        {
            rootVisualElement.RemoveFromClassList("Focused");
        }

        public void CreateGUI()
        {
            var window = windowTemplate.Instantiate();
            window.style.height = new StyleLength(Length.Percent(100)); // HACK: The template container doesn't fill the window for some reason.
            rootVisualElement.Add(window);
            scrollView = rootVisualElement.Q<ScrollView>();

            RefreshElements();
            SelectionHistoryData.instance.OnChanged += RefreshElements;
        }

        public void RefreshElements()
        {
            SelectionHistoryData data = SelectionHistoryData.instance;

            // Create additional visual elements if there aren't enough to show all of the histroy.
            while (data.History.Count > elements.Count)
            {
                var tree = elementTemplate.Instantiate();
                scrollView.Add(tree);
                elements.Add(tree);
                tree.RegisterCallback<ClickEvent, VisualElement>(OnElementClicked, tree);
                tree.RegisterCallback<DragPerformEvent, VisualElement>(OnDraggedIntoElement, tree);
                tree.AddManipulator(new SelectionHistoryDragAndDropManipulator(ElementToObject));
            }

            for (int i = 0; i < elements.Count; i++)
            {
                var element = elements[i];
                if (i >= data.History.Count)
                {
                    element.style.display = DisplayStyle.None;
                    continue;
                }

                var historyIndex = ElementIndexToHistoryIndex(i);
                var elementObj = data.History[historyIndex];
                var isSelected = historyIndex == SelectionHistoryData.instance.NavigationIndex;
                var isSceneObj = !EditorUtility.IsPersistent(elementObj);
                element.style.display = DisplayStyle.Flex;
                element.Q<Label>("ObjectLabel").text = elementObj.name;
                element.Q<Image>("ObjectIcon").image = AssetPreview.GetMiniThumbnail(elementObj);
                SetClass(element, "Selected", isSelected);
                SetClass(element, "SceneObj", isSceneObj);
                SetClass(element, "Pinned", false);
            }

            // Scroll to the top of the window if a new item was selected.
            if (Selection.activeObject == data.lastNonPinnedElement)
            {
                var scroller = scrollView.verticalScroller;
                scroller.value = scroller.lowValue;
            }
        }

        private void SetClass(VisualElement element, string ussClass, bool value)
        {
            if (value)
                element.AddToClassList(ussClass);
            else
                element.RemoveFromClassList(ussClass);
        }

        private void OnElementClicked(ClickEvent evt, VisualElement element)
        {
            Object obj = ElementToObject(element);
            if (Selection.activeObject == obj)
            {
                SelectionHistoryData.instance.SelectObjectFromHistory(obj);
            }
        }

        private void OnDraggedIntoElement(DragPerformEvent evt, VisualElement element)
        {
            var targetObject = ElementToObject(element);
            if (EditorUtility.IsPersistent(targetObject))
            {
                return; // Don't drop scene objects into assets.
            }

            foreach (var droppedObject in DragAndDrop.objectReferences)
            {
                if (!EditorUtility.IsPersistent(droppedObject))
                {
                    var a = droppedObject as GameObject;
                    var b = targetObject as GameObject;
                    if (a && b)
                    {
                        a.transform.parent = b.transform;
                    }
                }
            }
        }

        public static Object ElementToObject(VisualElement element)
        {
            int elementIndex = element.parent.IndexOf(element);
            var historyIndex = ElementIndexToHistoryIndex(elementIndex);
            return SelectionHistoryData.instance.History[historyIndex];
        }

        public static int ElementIndexToHistoryIndex(int i)
        {
            return SelectionHistoryData.instance.History.Count - 1 - i;
        }

        public void AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("History Preferences"), false, () => { SettingsService.OpenUserPreferences("Preferences/Inspector History"); });
        }
    }
}
