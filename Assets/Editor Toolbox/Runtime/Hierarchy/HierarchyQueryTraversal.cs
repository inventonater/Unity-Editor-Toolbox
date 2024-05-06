using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Toolbox
{
    public interface ITraversalAlgorithm<T> where T : Component
    {
        IEnumerable<T> Traverse(HierarchyQuery<T> query);
    }

    public enum ETraversalAlgorithm
    {
        DescendantsBreadthFirst,
        DescendantsDepthFirst,
        Ancestors,
        AncestorsTopDown,
        SameTreeLevel,
        Compound,
        TraverseFindObjectsOfType
    }


    public static class HierarchyQueryTraversal<T> where T : Component
    {
        public static ITraversalAlgorithm<T> Create(ETraversalAlgorithm algorithm)
        {
            return algorithm switch
            {
                ETraversalAlgorithm.DescendantsBreadthFirst => new DescendantsBreadthFirst(),
                ETraversalAlgorithm.DescendantsDepthFirst => new DescendantsDepthFirst(),
                ETraversalAlgorithm.Ancestors => new Ancestors(),
                ETraversalAlgorithm.AncestorsTopDown => new AncestorsTopDown(),
                ETraversalAlgorithm.SameTreeLevel => new SameTreeLevel(),
                ETraversalAlgorithm.Compound => new Compound(),
                ETraversalAlgorithm.TraverseFindObjectsOfType => new FindObjectsOfType(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public class DescendantsBreadthFirst : ITraversalAlgorithm<T>
        {
            private readonly Queue<Transform> _queue = new();

            public IEnumerable<T> Traverse(HierarchyQuery<T> query)
            {
                _queue.Clear();
                _queue.Enqueue(query.Origin.transform);
                while (_queue.Count > 0)
                {
                    var current = _queue.Dequeue();
                    for (int i = 0; i < current.childCount; i++)
                    {
                        _queue.Enqueue(current.GetChild(i));
                    }
                    foreach (var component in query.VisitComponents(current))
                    {
                        yield return component;
                    }
                }
            }
        }

        public class DescendantsDepthFirst : ITraversalAlgorithm<T>
        {
            private readonly Stack<Transform> _stack = new();

            public IEnumerable<T> Traverse(HierarchyQuery<T> query)
            {
                _stack.Clear();
                _stack.Push(query.Origin.transform);
                while (_stack.Count > 0)
                {
                    var current = _stack.Pop();
                    foreach (var component in query.VisitComponents(current))
                    {
                        yield return component;
                    }
                    for (int i = current.childCount - 1; i >= 0; i--)
                    {
                        _stack.Push(current.GetChild(i));
                    }
                }
            }
        }

        public class Ancestors : ITraversalAlgorithm<T>
        {
            public IEnumerable<T> Traverse(HierarchyQuery<T> query)
            {
                var current = query.Origin.transform; // Start from the origin object so we can potentially include it
                while (current != null)
                {
                    foreach (var component in query.VisitComponents(current)) yield return component;
                    current = current.parent;
                }
            }
        }

        public class AncestorsTopDown : ITraversalAlgorithm<T>
        {
            private readonly List<Transform> _ancestorsCache = new();

            public IEnumerable<T> Traverse(HierarchyQuery<T> query)
            {
                var current = query.Origin.transform;
                _ancestorsCache.Clear();

                while (current != null)
                {
                    _ancestorsCache.Add(current);
                    current = current.parent;
                }

                for (int i = _ancestorsCache.Count - 1; i >= 0; i--)
                {
                    current = _ancestorsCache[i];
                    foreach (var component in query.VisitComponents(current))
                    {
                        yield return component;
                    }
                }
            }
        }

        public class SameTreeLevel : ITraversalAlgorithm<T>
        {
            public IEnumerable<T> Traverse(HierarchyQuery<T> query)
            {
                var sameLevel = query.Origin.transform.parent != null ? query.Origin.transform.parent.Cast<Transform>() : Hierarchy.GetRootGameObject();
                foreach (Transform current in sameLevel)
                {
                    foreach (var component in query.VisitComponents(current)) yield return component;
                }
            }
        }

        public class Compound : ITraversalAlgorithm<T>
        {
            private readonly ITraversalAlgorithm<T>[] _traversals;

            public Compound(params ITraversalAlgorithm<T>[] traversals) => _traversals = traversals;

            public IEnumerable<T> Traverse(HierarchyQuery<T> query)
            {
                foreach (var traversal in _traversals)
                {
                    foreach (var component in traversal.Traverse(query)) yield return component;
                }
            }
        }

        public class FindObjectsOfType : ITraversalAlgorithm<T>
        {
            public IEnumerable<T> Traverse(HierarchyQuery<T> query)
            {
                var inScene = GameObject.FindObjectsOfType<T>(includeInactive: true);
                foreach (var component in inScene)
                {
                    if (query.QueryTraversal.ShouldSkip(component.gameObject)) continue;
                    if (query.QueryTraversal.ShouldSkip(component)) continue;
                    if (query.Filter.MatchesFilters(query.Origin, component)) yield return component;
                }
            }
        }
    }
}