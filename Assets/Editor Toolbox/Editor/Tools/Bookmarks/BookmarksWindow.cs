using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Toolbox.Editor
{
    public class BookmarksWindow : EditorWindow
    {
        public VisualTreeAsset windowTemplate;
        public VisualTreeAsset elementTemplate;
        private ScrollView scrollView;
        private List<VisualElement> elements = new List<VisualElement>();

        [MenuItem(ToolsMenu.MenuRoot + "/Bookmarks", priority = ToolsMenu.Priority)]
        public static void ShowWindow() {
            BookmarksWindow window = EditorWindow.GetWindow<BookmarksWindow>();
            window.titleContent = new GUIContent("Bookmarks");
            window.RefreshElements(); // Do refresh the list to highlight the new selection.
        }

        public void CreateGUI()
        {
            var window = windowTemplate.Instantiate();
            window.style.height = new StyleLength(Length.Percent(100));
            rootVisualElement.Add(window);
            rootVisualElement.RegisterCallback<DragPerformEvent, VisualElement>(OnDragPerformEvent, window);

            scrollView = rootVisualElement.Q<ScrollView>();
            RefreshElements();
            BookmarksData.instance.OnChanged += RefreshElements;
        }

        public void RefreshElements()
        {
            BookmarksData data = BookmarksData.instance;

            while (data.Bookmarks.Count > elements.Count)
            {
                var tree = elementTemplate.Instantiate();
                scrollView.Add(tree);
                elements.Add(tree);
                tree.RegisterCallback<ClickEvent, VisualElement>(OnElementClicked, tree);
                tree.RegisterCallback<KeyDownEvent, VisualElement>(OnKeyDown, tree);
                tree.RegisterCallback<DragPerformEvent, VisualElement>(OnDragPerformEvent, tree);
                tree.AddManipulator(new SelectionHistoryDragAndDropManipulator(ElementToObject));
            }

            for (int i = 0; i < elements.Count; i++)
            {
                var element = elements[i];
                if (i >= data.Bookmarks.Count)
                {
                    element.style.display = DisplayStyle.None;
                    continue;
                }
                var elementObj = data.Bookmarks[i];
                element.style.display = DisplayStyle.Flex;
                element.Q<Label>("ObjectLabel").text = elementObj.name;
                element.Q<Image>("ObjectIcon").image = AssetPreview.GetMiniThumbnail(elementObj);
                element.Q<Button>("RemoveButton").clicked += () => RemoveBookmark(elementObj);
            }
        }

        private void OnDragPerformEvent(DragPerformEvent evt, VisualElement element)
        {
            Debug.Log("OnDragPerformEvent");
            foreach (var droppedObject in DragAndDrop.objectReferences)
            {
                BookmarksData.instance.Add(droppedObject);
            }
        }

        private void OnElementClicked(ClickEvent evt, VisualElement element)
        {
            Object obj = ElementToObject(element);
            Selection.activeObject = obj;
        }

        private void OnKeyDown(KeyDownEvent evt, VisualElement element)
        {
            if (evt.keyCode == KeyCode.Delete)
            {
                Object obj = ElementToObject(element);
                RemoveBookmark(obj);
            }
        }

        private void RemoveBookmark(Object obj)
        {
            BookmarksData.instance.Remove(obj);
        }

        public static Object ElementToObject(VisualElement element)
        {
            int elementIndex = element.parent.IndexOf(element);
            return BookmarksData.instance.Bookmarks[elementIndex];
        }
    }
}
