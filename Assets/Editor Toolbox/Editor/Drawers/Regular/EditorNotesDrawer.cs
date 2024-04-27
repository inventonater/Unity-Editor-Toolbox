using System;
using Toolbox.Editor.Drawers;
using UnityEditor;
using UnityEngine;

namespace Toolbox
{

    /// <summary>
    /// Custom property drawer for the Note class.
    /// </summary>
    [CustomPropertyDrawer(typeof(Note))]
    public class EditorNotesDrawer : PropertyDrawerBase
    {
        private readonly GUIStyle _textAreaGuiStyle = new(EditorStyles.textArea) { wordWrap = true };

        /// <summary>
        /// Gets the SerializedProperty for the lastModified field of the Note.
        /// </summary>
        /// <param name="property">The SerializedProperty of the Note.</param>
        /// <returns>The SerializedProperty of the lastModified field.</returns>
        private static SerializedProperty GetLastModifiedProp(SerializedProperty property) => property.FindPropertyRelative($"{nameof(Note.lastModified)}.ticks");

        /// <summary>
        /// Gets the SerializedProperty for the body field of the Note.
        /// </summary>
        /// <param name="property">The SerializedProperty of the Note.</param>
        /// <returns>The SerializedProperty of the body field.</returns>
        private static SerializedProperty GetBodyProp(SerializedProperty property) => property.FindPropertyRelative(nameof(Note.body));

        /// <summary>
        /// Draws the custom GUI for the Note property.
        /// </summary>
        /// <param name="position">The position rectangle for the property.</param>
        /// <param name="property">The SerializedProperty of the Note.</param>
        /// <param name="label">The label for the property.</param>
        protected override void OnGUISafe(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty bodyProp = GetBodyProp(property);
            SerializedProperty lastModifiedProp = GetLastModifiedProp(property);

            const int paddingTop = 2;
            Rect labelRect = new Rect(position.x, position.y + paddingTop, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelRect, "Notes");

            using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
            {
                string bodyValue = EditorGUI.TextArea(TextAreaRect(position), bodyProp.stringValue, _textAreaGuiStyle);
                if (changeCheckScope.changed)
                {
                    bodyProp.stringValue = bodyValue;
                    lastModifiedProp.longValue = DateTime.Now.Ticks;
                    property.serializedObject.ApplyModifiedProperties();
                }
            }

            string lastModifiedText = $"Last modified: {new DateTime(lastModifiedProp.longValue):g}";
            GUIContent lastModifiedContent = new GUIContent(lastModifiedText);
            GUIStyle lastModifiedStyle = new GUIStyle(EditorStyles.miniLabel);
            lastModifiedStyle.normal.textColor = Color.gray;
            lastModifiedStyle.fontSize = 9;

            Vector2 lastModifiedSize = lastModifiedStyle.CalcSize(lastModifiedContent);

            Rect lastModifiedRect = new Rect(
                position.x + position.width - lastModifiedSize.x - 4,
                position.y + position.height - EditorGUIUtility.singleLineHeight,
                lastModifiedSize.x,
                EditorGUIUtility.singleLineHeight
            );
            EditorGUI.LabelField(lastModifiedRect, lastModifiedText, lastModifiedStyle);
        }

        /// <summary>
        /// Calculates the rectangle for the text area.
        /// </summary>
        /// <param name="position">The position rectangle for the property.</param>
        /// <returns>The rectangle for the text area.</returns>
        private static Rect TextAreaRect(Rect position) => new(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, position.height - EditorGUIUtility.singleLineHeight);

        /// <summary>
        /// Calculates the height of the property based on the content of the note.
        /// </summary>
        /// <param name="property">The SerializedProperty of the Note.</param>
        /// <param name="label">The label for the property.</param>
        /// <returns>The calculated height of the property.</returns>
        protected override float GetPropertyHeightSafe(SerializedProperty property, GUIContent label)
        {
            var bodyGuiContent = new GUIContent(GetBodyProp(property).stringValue);
            float textAreaWidth = EditorGUIUtility.currentViewWidth - 40;

            float height = _textAreaGuiStyle.CalcHeight(bodyGuiContent, textAreaWidth);
            return height + EditorGUIUtility.singleLineHeight + 10;
        }
    }
}