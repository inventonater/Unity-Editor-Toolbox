using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Toolbox.Editor.Hierarchy
{
    //TODO: refactor

    /// <summary>
    /// Base class for all custom, Hierarchy-related labels based on targeted <see cref="GameObject"/>.
    /// </summary>
    public abstract class HierarchyPropertyLabel
    {
        protected GameObject target;

        public virtual bool Prepare(GameObject target, Rect availableRect)
        {
            return this.target = target;
        }

        public virtual bool Prepare(GameObject target, Rect availableRect, out float neededWidth)
        {
            if (Prepare(target, availableRect))
            {
                neededWidth = GetWidth();
                return true;
            }
            else
            {
                neededWidth = 0.0f;
                return false;
            }
        }

        public virtual float GetWidth()
        {
            return Style.minWidth;
        }

        public abstract void OnGui(Rect rect);


        /// <summary>
        /// Returns built-in label class associated to provided <see cref="HierarchyItemDataType"/>.
        /// </summary>
        public static HierarchyPropertyLabel GetPropertyLabel(HierarchyItemDataType dataType)
        {
            switch (dataType)
            {
                case HierarchyItemDataType.Icon:
                    return new HierarchyIconLabel();
                case HierarchyItemDataType.Toggle:
                    return new HierarchyToggleLabel();
                case HierarchyItemDataType.Tag:
                    return new HierarchyTagLabel();
                case HierarchyItemDataType.Layer:
                    return new HierarchyLayerLabel();
                case HierarchyItemDataType.Script:
                    return new HierarchyScriptLabel();
                case HierarchyItemDataType.ChildCount:
                    return new HierarchyChildCountLabel();
            }

            return null;
        }

        #region Classes: Internal

        private class HierarchyIconLabel : HierarchyPropertyLabel
        {
            public override void OnGui(Rect rect)
            {
                var content = EditorGuiUtility.GetObjectContent(target, typeof(GameObject));
                if (content.image)
                {
                    GUI.Label(rect, content.image);
                }
            }
        }

        private class HierarchyChildCountLabel : HierarchyPropertyLabel
        {
            public override void OnGui(Rect rect)
            {
                var childCount = target.transform.childCount;
                var descendantCount = target.transform.DescendantCount();
                var tooltip = $"{childCount} children, {descendantCount} descendants";
                var content = new GUIContent(descendantCount.ToString(), tooltip);
                EditorGUI.LabelField(rect, content, Style.centreAlignTextStyle);
            }
        }


        private class HierarchyToggleLabel : HierarchyPropertyLabel
        {
            public override void OnGui(Rect rect)
            {
                var content = new GUIContent(string.Empty, "Enable/disable GameObject");

                //NOTE: using EditorGUI.Toggle will cause bug and deselect all hierarchy toggles when you will pick a multi-selected property in the Inspector
                var toggleRect = rect.WithX(rect.x + EditorGUIUtility.standardVerticalSpacing);
                var result = GUI.Toggle(toggleRect, target.activeSelf, content);

                if (rect.Contains(Event.current.mousePosition))
                {
                    if (result != target.activeSelf)
                    {
                        Undo.RecordObject(target, "SetActive");
                        target.SetActive(result);
                    }
                }
            }
        }

        private class HierarchyTagLabel : HierarchyPropertyLabel
        {
            public override float GetWidth()
            {
                return Style.maxWidth;
            }

            public override void OnGui(Rect rect)
            {
                var content = new GUIContent(target.CompareTag("Untagged") ? string.Empty : target.tag, target.tag);
                EditorGUI.LabelField(rect, content, Style.defaultAlignTextStyle);
            }
        }

        private class HierarchyLayerLabel : HierarchyPropertyLabel
        {
            public override void OnGui(Rect rect)
            {
                var layerMask = target.layer;
                var layerName = LayerMask.LayerToName(layerMask);

                string GetContentText()
                {
                    switch (layerMask)
                    {
                        case 00: return string.Empty;
                        case 05: return layerName;
                        default: return layerMask.ToString();
                    }
                }

                var content = new GUIContent(GetContentText(), layerName + " layer");
                EditorGUI.LabelField(rect, content, Style.centreAlignTextStyle);
            }
        }

        private class HierarchyScriptLabel : HierarchyPropertyLabel
        {
            private static Texture componentIcon;
            private static Texture transformIcon;

            private float baseWidth;
            private float summWidth;

            private bool isRowHovered;

            /// <summary>
            /// Cached components of the last prepared <see cref="target"/>.
            /// </summary>
            private List<Component> cachedComponents = new();

            public override bool Prepare(GameObject target, Rect availableRect)
            {
                var isValid = base.Prepare(target, availableRect);
                if (!isValid) return false;

                baseWidth = Style.minWidth;
                isRowHovered = availableRect.Contains(Event.current.mousePosition);

                target.GetComponents(cachedComponents);
                summWidth = cachedComponents.Count > 1 ? cachedComponents.Count * baseWidth : baseWidth;

                componentIcon = componentIcon == null ? EditorGUIUtility.IconContent("cs Script Icon").image : componentIcon;
                transformIcon = transformIcon == null ? EditorGUIUtility.IconContent("Transform Icon").image : transformIcon;
                return true;
            }

            public override float GetWidth() => summWidth;

            public static readonly Dictionary<Component, Rect> ComponentToRect = new();
            private static Rect _drawRect;
            private static double _drawTime;

            public override void OnGui(Rect rect)
            {
                rect.xMin = rect.xMax - baseWidth;

                //draw tooltip based on all available components
                rect.xMin -= baseWidth * (cachedComponents.Count - 1);

                var currentIconRect = rect;
                currentIconRect.xMin = rect.xMin;
                currentIconRect.xMax = rect.xMin + baseWidth;

                //draw all icons associated to cached components (except transform)
                for (var i = cachedComponents.Count - 1; i >= 0; i--)
                {
                    var component = cachedComponents[i];
                    ComponentToRect.TryAdd(component, currentIconRect);

                    var iconTexture = EditorGUIUtility.ObjectContent(component, component.GetType()).image;
                    if (iconTexture == null) iconTexture = componentIcon;

                    var text = string.Empty;
                    var tooltip = component.GetType().Name;

                    if (component is Transform t)
                    {
                        var descendantCount = t.DescendantCount();
                        text = descendantCount.ToString();
                        tooltip = $"{t.childCount} children, {descendantCount} descendants";
                    }

                    //draw icon with tooltip for the current component

                    Color original = GUI.color;
                    var isComponentHovered = currentIconRect.Contains(Event.current.mousePosition);
                    if (isComponentHovered)
                    {
                        _drawRect = new Rect(currentIconRect.x, currentIconRect.y, 3, 90);
                        _drawTime = EditorApplication.timeSinceStartup;
                    }

                    GUI.color = GUI.color.WithAlpha(isComponentHovered ? 1f : 0.4f);
                    GUI.Label(currentIconRect, new GUIContent(iconTexture));
                    GUI.color = original;

                    var centreAlignTextStyle = Style.centreAlignTextStyle;
                    centreAlignTextStyle.fontStyle = FontStyle.Bold;
                    if (GUI.Button(currentIconRect, new GUIContent { text = text, tooltip = tooltip }, centreAlignTextStyle))
                    {
                        SelectComponentAndCollapseOthers(component);
                    }

                    //adjust rect for the next script icon
                    currentIconRect.x += baseWidth;

                    if (EditorApplication.timeSinceStartup - _drawTime < 0.05)
                    {
                        // use Graphics.DrawTexture to draw a texture in teh window
                        Graphics.DrawTexture(new Rect(100,100, 100, 100), EditorGUIUtility.whiteTexture);

                        EditorGUI.DrawRect(_drawRect, Color.red);
                    }
                }
            }
        }

        private void SelectComponentAndCollapseOthers(Object selectedComponent)
        {
            Debug.Log(selectedComponent);
            Selection.activeObject = selectedComponent;

            ActiveEditorTracker.sharedTracker.ForceRebuild();
            EditorWindow inspectorWindow = EditorWindow.GetWindow(typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.InspectorWindow"));
            ActiveEditorTracker.sharedTracker.ForceRebuild();

            UnityEditor.Editor[] editors = ActiveEditorTracker.sharedTracker.activeEditors;
            foreach (var editor in editors)
            {
                if (editor.GetType().Name == "TransformEditor") continue;
                InternalEditorUtility.SetIsInspectorExpanded(editor.target, editor.target == selectedComponent);
            }

            inspectorWindow.Repaint();
        }

        #endregion

        protected static class Style
        {
            internal static readonly float minWidth = 17.0f;
            internal static readonly float maxWidth = 60.0f;

            internal static readonly GUIStyle defaultAlignTextStyle;
            internal static readonly GUIStyle centreAlignTextStyle;
            internal static readonly GUIStyle rightAlignTextStyle;

            static Style()
            {
                defaultAlignTextStyle = new GUIStyle(EditorStyles.miniLabel)
                {
#if UNITY_2019_3_OR_NEWER
                    fontSize = 9
#else
                    fontSize = 8
#endif
                };
                centreAlignTextStyle = new GUIStyle(EditorStyles.miniLabel)
                {
#if UNITY_2019_3_OR_NEWER
                    fontSize = 9,
#else
                    fontSize = 8,
#endif
#if UNITY_2019_3_OR_NEWER
                    alignment = TextAnchor.MiddleCenter
#else
                    alignment = TextAnchor.UpperCenter
#endif
                };
                rightAlignTextStyle = new GUIStyle(EditorStyles.miniLabel)
                {
#if UNITY_2019_3_OR_NEWER
                    fontSize = 9,
#else
                    fontSize = 8,
#endif
#if UNITY_2019_3_OR_NEWER
                    alignment = TextAnchor.MiddleRight
#else
                    alignment = TextAnchor.UpperRight
#endif
                };
            }
        }
    }
}