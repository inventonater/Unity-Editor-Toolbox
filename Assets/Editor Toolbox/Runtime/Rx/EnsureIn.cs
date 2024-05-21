using System;
using System.Collections;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

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

            if (searchFlags.IsNone()) searchFlags = relation.ToRelationFlags();

            var query = searcher.Query<T>(searchFlags);

            T result = query.FirstOrDefault();
            if (result != null) return result;
            return TryCreate(searcher, relation, defaultType, out result) ? result : null;
        }

        // reimplement this with Rx
        public static IEnumerator YieldFor<T>(Component searcher, RelationFlags relationFlags, float timeout = 10f) where T : Component
        {
            var query = searcher.Query<T>(relationFlags);

            float startTime = Time.time;
            T result = null;

            while (result == null)
            {
                result = query.FirstOrDefault();
                if (result != null) yield break;

                if (Time.time - startTime > timeout)
                {
                    Debug.LogError($"Timed out waiting for dependency of type {typeof(T)}");
                    yield break;
                }

                yield return null;
            }
        }

        public static async UniTask<T> YieldForAsync<T>(Component searcher, RelationFlags relationFlags, float timeout = 10f, PlayerLoopTiming timing = PlayerLoopTiming.Update) where T : Component
        {
            var query = searcher.Query<T>(relationFlags);

            using var timeoutController = new TimeoutController();
            timeoutController.Timeout(TimeSpan.FromSeconds(timeout));
            while (!timeoutController.IsTimeout())
            {
                var result = query.FirstOrDefault();
                if (result != null) return result;
                await UniTask.Yield(timing);
            }

            Debug.LogError($"Timed out waiting for dependency of type {typeof(T)}");
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
