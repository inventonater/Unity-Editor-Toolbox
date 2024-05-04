using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Editor = UnityEditor.Editor;
using Object = UnityEngine.Object;

namespace Toolbox.Editor.Editors
{
    // [CustomEditor(typeof(Sample), true, isFallback = false)]
    [CanEditMultipleObjects]
    public class BehaviourInspectorEditor : ToolboxEditor
    {
        private const double UpdateThreshold = 0.2f;
        private double _lastUpdateTime;

        private void OnEnable()
        {
            EditorApplication.update += ThrottledUpdate;
        }

        private void OnDisable()
        {
            EditorApplication.update -= ThrottledUpdate;
        }

        private void ThrottledUpdate()
        {
            if (EditorApplication.timeSinceStartup - _lastUpdateTime < UpdateThreshold) return;
            _lastUpdateTime = EditorApplication.timeSinceStartup;
            EditorUtility.SetDirty(target);
        }

        public override void DrawCustomInspector()
        {
            base.DrawCustomInspector();

            Object myClass = (Object)target;

            // var propertyInfos = myClass.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            // foreach (var propertyInfo in propertyInfos.Where(p => p.Name.Contains("rx")))
            // {
            //     Rx<PopulatorTest> rxProp = propertyInfo.GetValue(myClass) as Rx<PopulatorTest>;
            //     string fieldName = propertyInfo.Name;
            //     EditorGUI.BeginChangeCheck();
            //     EditorGUILayout.LabelField(fieldName);
            //     Object objectField = EditorGUILayout.ObjectField(fieldName, rxProp.Value, rxProp.Value.GetType());
            //     if (EditorGUI.EndChangeCheck())
            //     {
            //         Debug.Log(objectField.GetType());
            //         rxProp.Value = (PopulatorTest)objectField;
            //     }
            // }

            // Get all private fields using reflection
            FieldInfo[] fields = myClass.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

            // Render each private field in the Inspector
            foreach (FieldInfo field in fields)
            {
                object value = field.GetValue(myClass);

                // Render the field based on its type
                string fieldName = field.Name;
                if (fieldName.EndsWith("__BackingField")) fieldName = fieldName.Substring(1, fieldName.IndexOf('>') - 1);

                if (field.FieldType == typeof(int))
                {
                    int intValue = (int)value;
                    intValue = EditorGUILayout.IntField(fieldName, intValue);
                    field.SetValue(myClass, intValue);
                }
                else if (field.FieldType == typeof(float))
                {
                    float floatValue = (float)value;
                    floatValue = EditorGUILayout.FloatField(fieldName, floatValue);
                    field.SetValue(myClass, floatValue);
                }
                else if (field.FieldType == typeof(string))
                {
                    string stringValue = (string)value;
                    stringValue = EditorGUILayout.TextField(fieldName, stringValue);
                    field.SetValue(myClass, stringValue);
                }
                else if (field.FieldType == typeof(bool))
                {
                    bool boolValue = (bool)value;
                    boolValue = EditorGUILayout.Toggle(fieldName, boolValue);
                    field.SetValue(myClass, boolValue);
                }
                else if (field.FieldType == typeof(Vector2))
                {
                    Vector2 vector2Value = (Vector2)value;
                    vector2Value = EditorGUILayout.Vector2Field(fieldName, vector2Value);
                    field.SetValue(myClass, vector2Value);
                }
                else if (field.FieldType == typeof(Vector3))
                {
                    Vector3 vector3Value = (Vector3)value;
                    vector3Value = EditorGUILayout.Vector3Field(fieldName, vector3Value);
                    field.SetValue(myClass, vector3Value);
                }
                else if (field.FieldType == typeof(Color))
                {
                    Color colorValue = (Color)value;
                    colorValue = EditorGUILayout.ColorField(fieldName, colorValue);
                    field.SetValue(myClass, colorValue);
                }
                else if (field.FieldType == typeof(Vector4))
                {
                    Vector4 vector4Value = (Vector4)value;
                    vector4Value = EditorGUILayout.Vector4Field(fieldName, vector4Value);
                    field.SetValue(myClass, vector4Value);
                }
                else if (field.FieldType == typeof(Rect))
                {
                    Rect rectValue = (Rect)value;
                    rectValue = EditorGUILayout.RectField(fieldName, rectValue);
                    field.SetValue(myClass, rectValue);
                }
                else if (field.FieldType == typeof(AnimationCurve))
                {
                    AnimationCurve animationCurveValue = (AnimationCurve)value;
                    animationCurveValue = EditorGUILayout.CurveField(fieldName, animationCurveValue);
                    field.SetValue(myClass, animationCurveValue);
                }
                else if (field.FieldType == typeof(Bounds))
                {
                    Bounds boundsValue = (Bounds)value;
                    boundsValue = EditorGUILayout.BoundsField(fieldName, boundsValue);
                    field.SetValue(myClass, boundsValue);
                }
                else if (field.FieldType == typeof(LayerMask))
                {
                    // need to allow for LayerMask flags
                    // LayerMask layerMaskValue = (LayerMask)value;
                    // layerMaskValue = EditorGUILayout.LayerField(fieldName, layerMaskValue);
                    // field.SetValue(myClass, layerMaskValue);
                }
                else if (field.FieldType == typeof(Gradient))
                {
                    Gradient gradientValue = (Gradient)value;
                    gradientValue = EditorGUILayout.GradientField(fieldName, gradientValue);
                    field.SetValue(myClass, gradientValue);
                }
                else if (field.FieldType.IsEnum)
                {
                    Enum enumValue = (Enum)value;
                    enumValue = EditorGUILayout.EnumPopup(fieldName, enumValue);
                    field.SetValue(myClass, enumValue);
                }
                else if (field.FieldType.IsArray)
                {
                    // EditorGUILayout.LabelField(fieldName);
                    // EditorGUI.indentLevel++;
                    // Array arrayValue = (Array)value;
                    // int newSize = EditorGUILayout.IntField("Size", arrayValue.Length);
                    // if (newSize != arrayValue.Length)
                    // {
                    //     System.Array.Resize(ref arrayValue, newSize);
                    //     field.SetValue(myClass, arrayValue);
                    // }
                    //
                    // for (int i = 0; i < arrayValue.Length; i++)
                    // {
                    //     EditorGUILayout.LabelField("Element " + i);
                    //     EditorGUI.indentLevel++;
                    //     arrayValue.SetValue(DrawField(arrayValue.GetValue(i)), i);
                    //     EditorGUI.indentLevel--;
                    // }
                    //
                    // EditorGUI.indentLevel--;
                }
            }

            // Apply changes if the script is modified
            if (GUI.changed)
            {
                EditorUtility.SetDirty(myClass);
            }
        }
    }
}
