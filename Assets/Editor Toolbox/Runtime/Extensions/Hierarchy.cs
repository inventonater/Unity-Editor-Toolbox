using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Component = UnityEngine.Component;

namespace Toolbox
{
    public static class Hierarchy
    {
        public static IEnumerable<Transform> GetRootGameObject()
        {
            return UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects().Select(go => go.transform);
        }

        public static int GetDepth(Component a)
        {
            int depth = 0;
            var t = a.transform;
            while (t != null)
            {
                depth++;
                t = t.parent;
            }

            return depth;
        }

        public enum HierarchySortRules
        {
            None,
            FirstInHierarchy,
            LastInHierarchy,
            RenderFirst,
            RenderLast
        }

        public static T Sort<T>(T a, T b, HierarchySortRules sortRules) where T : Component
        {
            if (sortRules == HierarchySortRules.FirstInHierarchy) return CompareBreadthFirst(a, b) < 0 ? a : b;
            if (sortRules == HierarchySortRules.LastInHierarchy) return CompareBreadthFirst(a, b) < 0 ? b : a;
            if (sortRules == HierarchySortRules.RenderFirst) return CompareInspectorOrder(a, b) < 0 ? a : b;
            if (sortRules == HierarchySortRules.RenderLast) return CompareInspectorOrder(a, b) < 0 ? b : a;
            Debug.LogWarning("Hierarchy.Find cannot use SearchOrder.None");
            return a;
        }

        public static int CompareBreadthFirst(Component a, Component b)
        {
            if (a == null || b == null) return 0;
            var aDepth = GetDepth(a);
            var bDepth = GetDepth(b);
            var depthCompare = aDepth.CompareTo(bDepth);
            return depthCompare != 0 ? depthCompare : CompareInspectorOrder(a, b);
        }

        private const bool DebugLog = false;

        private static readonly Stack<Transform> _ancestryA = new();
        private static readonly Stack<Transform> _ancestryB = new();

        // -1: a renders behind b.
        // 1: a renders after b.
        // First is rendered behind
        // Last is rendered ontop
        public static int CompareInspectorOrder(Component a, Component b)
        {
            if (a == null || b == null) return 0;

            void PopulateAncestry(Transform target, Stack<Transform> ancestry)
            {
                ancestry.Clear();
                ancestry.Push(target);
                while (target.parent != null)
                {
                    target = target.parent;
                    ancestry.Push(target);
                }
            }

            PopulateAncestry(a.transform, _ancestryA);
            PopulateAncestry(b.transform, _ancestryB);

            // no common root, cannot compare
            if (_ancestryA.Pop() != _ancestryB.Pop()) return 0;

            var minDepth = Mathf.Min(_ancestryA.Count, _ancestryB.Count);
            for (var i = 0; i < minDepth; i++)
            {
                var ancestorA = _ancestryA.Pop();
                var ancestorB = _ancestryB.Pop();

                // traverse down from shared root until the first branch is found
                if (ancestorA != ancestorB)
                {
                    var siblingIndexA = ancestorA.GetSiblingIndex();
                    var siblingIndexB = ancestorB.GetSiblingIndex();
                    var compareResult = siblingIndexA.CompareTo(siblingIndexB);
                    return compareResult;
                }
            }

            // same ancestry line, choose the deeper one is Last
            return _ancestryA.Count.CompareTo(_ancestryB.Count);
        }

        [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
        private static void Log(object message) => Debug.Log(message);
    }
}
