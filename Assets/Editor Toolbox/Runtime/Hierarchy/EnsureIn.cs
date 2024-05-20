using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Toolbox.Hierarchy;

namespace Toolbox
{
    public static class EnsureIn
    {
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

        // reimplement this with Rx
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
            var compoundAlgorithm = new HierarchyQueryAlgorithm<T>.Compound();

            foreach (var flag in search.GetFlags())
            {
                // should use a specific algorithm here.. Descendants is not specific enough?
                if (!SearchAlgorithms.TryGetValue(flag, out var foundAlgorithm)) continue;

                // todo need to manage order of algorithm execution somehow
                compoundAlgorithm.Add(foundAlgorithm);
            }

            var query = new Query<T>(searcher, algorithm);
            return query.FirstOrDefault();

            // foreach (var flag in search.GetFlags())
            // {
            //     if (!SearchFunctions.TryGetValue(flag, out var searchFunction)) continue;
            //     var result = searchFunction(searcher, typeof(T)) as T;
            //     if (result != null) return result;
            // }
            // return null;
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