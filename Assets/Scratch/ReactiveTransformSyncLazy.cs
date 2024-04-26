﻿using System;
using System.Reflection;
using System.Text.RegularExpressions;
using R3;
using UnityEditor;
using UnityEngine;

public static class MyExtensions
{
    public static IDisposable Subscribe<T>(this Component component, ReactiveProperty<T> property, Action<T> action)
    {
        return property.Subscribe(action).AddTo(component);
    }

    public static void Bind<TProperty>(this Component component, Func<Component, TProperty> get, Action<TProperty> set, ReactiveProperty<TProperty> property)
    {
        Observable.EveryValueChanged(component, get)
            .DistinctUntilChanged()
            .Subscribe(value => property.Value = value)
            .AddTo(component);
        component.Subscribe(property, set);
    }
}


[Serializable]
public class ReactivePropertySynchronizer<T> : SerializableReactiveProperty<T>, IDisposable
{
    private readonly Func<T> _getter;
    private readonly Action<T> _setter;
    private readonly CompositeDisposable _disposables = new();

    public ReactivePropertySynchronizer(Func<T> getter, Action<T> setter)
        : base(getter())
    {
        this._getter = getter;
        this._setter = setter;

        // Subscribe to changes in the getter and update the ReactiveProperty
        Observable.EveryUpdate()
            .Select(_ => getter())
            .DistinctUntilChanged()
            .Subscribe(value => Value = value)
            .AddTo(_disposables);

        // Subscribe to changes in the ReactiveProperty and update the setter
        this.Skip(1)
            .Subscribe(setter)
            .AddTo(_disposables);
    }

    public override void Dispose()
    {
        _disposables.Dispose();
        base.Dispose();
    }
}


public class ReactiveTransformSyncLazy : MonoBehaviour
{
    public bool EnableDebugLogging = true;

    private ReactivePropertySynchronizer<Vector3> _position;
    public ReactivePropertySynchronizer<Vector3> Position => LazyInitialize(ref _position, () => transform.position, value => transform.position = value);

    private ReactivePropertySynchronizer<Quaternion> _rotation;
    public ReactivePropertySynchronizer<Quaternion> Rotation => LazyInitialize(ref _rotation, () => transform.rotation, value => transform.rotation = value);

    private ReactivePropertySynchronizer<Vector3> _scale;
    public ReactivePropertySynchronizer<Vector3> Scale => LazyInitialize(ref _scale, () => transform.localScale, value => transform.localScale = value);

    private ReactivePropertySynchronizer<Vector2> _anchorMin;
    public ReactivePropertySynchronizer<Vector2> AnchorMin => LazyInitialize(ref _anchorMin, () => GetRectTransform().anchorMin, value => GetRectTransform().anchorMin = value);

    private ReactivePropertySynchronizer<Vector2> _anchorMax;
    public ReactivePropertySynchronizer<Vector2> AnchorMax => LazyInitialize(ref _anchorMax, () => GetRectTransform().anchorMax, value => GetRectTransform().anchorMax = value);

    private ReactivePropertySynchronizer<Vector2> _pivot;
    public ReactivePropertySynchronizer<Vector2> Pivot => LazyInitialize(ref _pivot, () => GetRectTransform().pivot, value => GetRectTransform().pivot = value);

    private ReactivePropertySynchronizer<Vector2> _sizeDelta;
    public ReactivePropertySynchronizer<Vector2> SizeDelta => LazyInitialize(ref _sizeDelta, () => GetRectTransform().sizeDelta, value => GetRectTransform().sizeDelta = value);

    private ReactivePropertySynchronizer<Vector2> _anchoredPosition;
    public ReactivePropertySynchronizer<Vector2> AnchoredPosition => LazyInitialize(ref _anchoredPosition, () => GetRectTransform().anchoredPosition, value => GetRectTransform().anchoredPosition = value);

    private readonly CompositeDisposable _disposables = new CompositeDisposable();

    private ReactivePropertySynchronizer<TValue> LazyInitialize<TValue>(ref ReactivePropertySynchronizer<TValue> backingField, Func<TValue> getter, Action<TValue> setter)
    {
        if (backingField != null) return backingField;

        backingField = new ReactivePropertySynchronizer<TValue>(getter, setter);
        if (EnableDebugLogging)
        {
            backingField
                .Subscribe(value => LogReactivePropertyChange(typeof(TValue).Name, value))
                .AddTo(_disposables);
        }

        return backingField;
    }

    private RectTransform GetRectTransform()
    {
        if (TryGetComponent(out RectTransform rt)) return rt;
        throw new InvalidOperationException("RectTransform component not found.");
    }

    private void LogReactivePropertyChange<TValue>(string propertyName, TValue value)
    {
        Debug.Log($"{propertyName} changed: {value}");
    }

    private void OnDestroy()
    {
        _disposables.Dispose();
    }
}

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(ReactivePropertySynchronizer<>))]
internal class ReactivePropertySynchronizerDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var p = property.FindPropertyRelative("value");

        EditorGUI.BeginChangeCheck();

        if (p.propertyType == SerializedPropertyType.Quaternion)
        {
            label.text += "(EulerAngles)";
            EditorGUI.PropertyField(position, p, label, true);
        }
        else
        {
            EditorGUI.PropertyField(position, p, label, true);
        }

        if (EditorGUI.EndChangeCheck())
        {
            var paths = property.propertyPath.Split('.'); // X.Y.Z...
            var attachedComponent = property.serializedObject.targetObject;

            var targetProp = (paths.Length == 1)
                ? fieldInfo.GetValue(attachedComponent)
                : GetValueRecursive(attachedComponent, 0, paths);
            if (targetProp == null) return;

            property.serializedObject.ApplyModifiedProperties(); // deserialize to field
            var methodInfo = targetProp.GetType().GetMethod("ForceNotify",
                BindingFlags.IgnoreCase | BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (methodInfo != null)
            {
                methodInfo.Invoke(targetProp, Array.Empty<object>());
            }
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var p = property.FindPropertyRelative("value");
        if (p.propertyType == SerializedPropertyType.Quaternion)
        {
            // Quaternion is Vector3(EulerAngles)
            return EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector3, label);
        }
        else
        {
            return EditorGUI.GetPropertyHeight(p);
        }
    }

    object GetValueRecursive(object obj, int index, string[] paths)
    {
        var path = paths[index];

        FieldInfo fldInfo = null;
        var type = obj.GetType();
        while (fldInfo == null)
        {
            // attempt to get information about the field
            fldInfo = type.GetField(path, BindingFlags.IgnoreCase | BindingFlags.GetField | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (fldInfo != null ||
                type.BaseType == null ||
                type.BaseType.IsSubclassOf(typeof(ReactiveProperty<>))) break;

            // if the field information is missing, it may be in the base class
            type = type.BaseType;
        }

        // If array, path = Array.data[index]
        if (fldInfo == null && path == "Array")
        {
            try
            {
                path = paths[++index];
                var m = Regex.Match(path, @"(.+)\[([0-9]+)*\]");
                var arrayIndex = int.Parse(m.Groups[2].Value);
                var arrayValue = (obj as System.Collections.IList)[arrayIndex];
                if (index < paths.Length - 1)
                {
                    return GetValueRecursive(arrayValue, ++index, paths);
                }
                else
                {
                    return arrayValue;
                }
            }
            catch
            {
                Debug.Log("ReactivePropertySynchronizerDrawer Exception, objType:" + obj.GetType().Name + " path:" + string.Join(", ", paths));
                throw;
            }
        }
        else if (fldInfo == null)
        {
            throw new Exception("Can't decode path:" + string.Join(", ", paths));
        }

        var v = fldInfo.GetValue(obj);
        if (index < paths.Length - 1)
        {
            return GetValueRecursive(v, ++index, paths);
        }

        return v;
    }
}

#endif