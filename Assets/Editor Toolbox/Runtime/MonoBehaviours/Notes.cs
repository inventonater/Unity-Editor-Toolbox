using UnityEditor;
using UnityEngine;

namespace Toolbox
{
    public class Notes : MonoBehaviour
    {
        public Note notes;

        // [Header("Note Properties")]
        // [SerializeField] private string title; // Title for the note
        // [TextArea(3, 10), SerializeField] private string body; // Body of the note
        // [SerializeField] private Color noteColor = Color.white; // Color for the note background
        //
        // // Timestamps for creation and last modification
        // private string createdAt;
        // private string lastModified;
        //
        // void Reset()
        // {
        //     createdAt = System.DateTime.Now.ToString();
        //     lastModified = createdAt;
        // }
        //
        // void OnValidate()
        // {
        //     lastModified = System.DateTime.Now.ToString();
        // }

//
// #if UNITY_EDITOR
//         [CustomEditor(typeof(Notes))]
//         public class NotesEditor : Editor
//         {
//             public override void OnInspectorGUI()
//             {
//                 var notes = target as Notes;
//
//                 EditorGUILayout.BeginVertical(EditorStyles.helpBox);
//                 EditorGUI.indentLevel++;
//
//                 // Drawing title field
//                 notes.title = EditorGUILayout.TextField("Title", notes.title);
//
//                 // Drawing color field
//                 notes.noteColor = EditorGUILayout.ColorField("Note Color", notes.noteColor);
//
//                 // Drawing text area for body
//                 notes.body = EditorGUILayout.TextArea(notes.body, GUILayout.MinHeight(60));
//
//                 EditorGUI.indentLevel--;
//                 EditorGUILayout.EndVertical();
//
//                 EditorGUILayout.LabelField("Created At: " + notes.createdAt);
//                 EditorGUILayout.LabelField("Last Modified: " + notes.lastModified);
//
//                 if (GUI.changed)
//                 {
//                     EditorUtility.SetDirty(notes);
//                 }
//             }
//         }
// #endif
    }
}