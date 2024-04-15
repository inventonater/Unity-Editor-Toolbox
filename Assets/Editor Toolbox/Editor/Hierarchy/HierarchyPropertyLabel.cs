using System.Collections.Generic;
using System.Text;
using UnityEditor;
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

        private class HierarchyToggleLabel : HierarchyPropertyLabel
        {
            public override void OnGui(Rect rect)
            {
                var content = new GUIContent(string.Empty, "Enable/disable GameObject");
                //NOTE: using EditorGUI.Toggle will cause bug and deselect all hierarchy toggles when you will pick a multi-selected property in the Inspector
                var result = GUI.Toggle(new Rect(rect.x + EditorGUIUtility.standardVerticalSpacing,
                        rect.y,
                        rect.width,
                        rect.height),
                    target.activeSelf, content);

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

            private bool isHighlighted;

            /// <summary>
            /// Cached components of the last prepared <see cref="target"/>.
            /// </summary>
            private List<Component> cachedComponents = new();

            public override bool Prepare(GameObject target, Rect availableRect)
            {
                var isValid = base.Prepare(target, availableRect);
                if (!isValid) return false;

                baseWidth = Style.minWidth;
                isHighlighted = availableRect.Contains(Event.current.mousePosition);

                target.GetComponents(cachedComponents);
                summWidth = cachedComponents.Count > 1 ? (cachedComponents.Count - 1) * baseWidth : baseWidth;

                componentIcon = componentIcon == null ? EditorGUIUtility.IconContent("cs Script Icon").image : componentIcon;
                transformIcon = transformIcon == null ? EditorGUIUtility.IconContent("Transform Icon").image : transformIcon;
                return true;
            }

            public override float GetWidth() => summWidth;

            public override void OnGui(Rect rect)
            {
                rect.xMin = rect.xMax - baseWidth;

                //draw tooltip based on all available components
                rect.xMin -= baseWidth * (cachedComponents.Count - 1);

                var iconRect = rect;
                iconRect.xMin = rect.xMin;
                iconRect.xMax = rect.xMin + baseWidth;

                //draw all icons associated to cached components (except transform)
                for (var i = cachedComponents.Count - 1; i >= 0; i--)
                {
                    var cached = cachedComponents[i];
                    var iconTexture = EditorGUIUtility.ObjectContent(cached, cached.GetType()).image;
                    if (iconTexture == null) iconTexture = componentIcon;

                    //draw icon for the current component
                    GUI.Label(iconRect, new GUIContent(iconTexture));
                    GUI.Label(iconRect, new GUIContent
                    {
                        tooltip = cached.GetType().Name
                    });

                    //adjust rect for the next script icon
                    iconRect.x += baseWidth;
                }
            }
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
