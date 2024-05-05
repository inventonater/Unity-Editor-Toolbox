using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = FileName, menuName = MenuName)]
public class PromptTemplate : ScriptableObject
{
    public const string FileName = nameof(AiScribe) + " Prompt";
    public const string MenuName = nameof(AiScribe) + "/New Prompt Templates";
    public string text = "Enter your custom prompt here...";
    public string notes = "";

    public AiScribe.Options options;

    [CustomEditor(typeof(PromptTemplate))]
    public class PromptTemplateEditor : Editor
    {
        private Vector2 scrollPosition;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(options)));

            EditorGUILayout.Space();

            SerializedProperty textProp = serializedObject.FindProperty(nameof(text));
            SerializedProperty notesProp = serializedObject.FindProperty(nameof(notes));

            GUIStyle textAreaStyle = new GUIStyle(GUI.skin.textArea) { wordWrap = true };

            EditorGUI.BeginChangeCheck();
            string newText = EditorGUILayout.TextArea(textProp.stringValue, textAreaStyle, GUILayout.ExpandHeight(true));
            string newNotes = EditorGUILayout.TextArea(notesProp.stringValue, textAreaStyle, GUILayout.Height(200));
            if (EditorGUI.EndChangeCheck())
            {
                textProp.stringValue = newText;
                notesProp.stringValue = newNotes;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }

    // Hack to allow an empty first entry in the "Select Prompt Template" dropdown
    public string Title => this.name;

}