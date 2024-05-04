using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolbox
{
    public static class EnsureIn
    {
        private static readonly Dictionary<RelationFlags, Func<Component, Type, Component>> SearchFunctions = new()
        {
            { RelationFlags.Sibling, (searcher, type) => searcher.GetComponent(type) },
            { RelationFlags.Parent, (searcher, type) => searcher.transform.parent.GetComponent(type) },
            { RelationFlags.Child, (searcher, type) => GetChildComponent(searcher, type) },
            { RelationFlags.Ancestor, (searcher, type) => searcher.GetComponentInParent(type) },
            { RelationFlags.Descendant, (searcher, type) => searcher.GetComponentInChildren(type, true) }
        };

        private static Component GetChildComponent(Component searcher, Type type)
        {
            for (int i = 0; i < searcher.transform.childCount; i++)
            {
                var child = searcher.transform.GetChild(i);
                var component = child.GetComponent(type);
                if (component != null) return component;
            }
            return null;
        }

        public static T Ensure<T>(this Component searcher, ref T field, Relation relation = Relation.Sibling, RelationFlags searchFlags = RelationFlags.None, Type defaultType = null) where T : Component
        {
            if (field != null) return field;
            field = Ensure<T>(searcher, relation, searchFlags, defaultType);
            return field;
        }

        public static T Ensure<T>(this Component searcher, Relation relation = Relation.Sibling, RelationFlags searchFlags = RelationFlags.None, Type defaultType = null) where T : Component
        {
            if (searcher == null)
            {
                throw new ArgumentNullException(nameof(searcher), $"Ensure<{typeof(T)}> called with null searcher");
            }

            if (searchFlags.IsNone()) searchFlags = relation.ToFlags();

            T result = Search<T>(searcher, searchFlags);
            if (result != null) return result;

            return TryCreate<T>(searcher, relation, defaultType, out result) ? result : null;
        }

        public static IEnumerator WaitForDependency<T>(Component searcher, RelationFlags search, float timeout = 10f) where T : Component
        {
            float startTime = Time.time;
            T result = null;

            while (result == null)
            {
                result = Search<T>(searcher, search);
                if (result != null) yield break;

                if (Time.time - startTime > timeout)
                {
                    Debug.LogError($"Timed out waiting for dependency of type {typeof(T)}");
                    yield break;
                }

                yield return null;
            }
        }

        public static T Search<T>(this Component searcher, RelationFlags search) where T : Component
        {
            foreach (var flag in search.GetFlags())
            {
                if (!SearchFunctions.TryGetValue(flag, out var searchFunction)) continue;

                var result = searchFunction(searcher, typeof(T)) as T;
                if (result != null) return result;
            }

            return null;
        }

        private static bool TryCreate<T>(Component searcher, Relation relation, Type defaultType, out T result) where T : Component
        {
            Transform host = relation switch
            {
                Relation.Child => searcher.transform,
                Relation.Descendant => searcher.transform,
                Relation.Sibling => searcher.transform,
                Relation.Parent => searcher.transform.parent,
                Relation.Ancestor => searcher.transform.root,
                _ => null
            };

            if (host == null)
            {
                result = searcher.CreateChild<T>(defaultType);
                result.transform.SetParent(null);
                return true;
            }

            result = defaultType != null ? host.gameObject.AddComponent(defaultType) as T : host.gameObject.AddComponent<T>();
            return true;
        }
    }
}