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
        TraverseFindObjectsOfType,
        ImmediateChildren
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
                ETraversalAlgorithm.ImmediateChildren => new ImmediateChildren(),
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
                    foreach (var component in query.VisitComponents(current))
                    {
                        yield return component;
                    }

                    for (int i = 0; i < current.childCount; i++)
                    {
                        _queue.Enqueue(current.GetChild(i));
                    }
                }
            }
        }

        public class DescendantsDepthFirst : ITraversalAlgorithm<T>
        {
            public IEnumerable<T> Traverse(HierarchyQuery<T> query)
            {
                return TraverseRecursive(query.Origin.transform);

                IEnumerable<T> TraverseRecursive(Transform current)
                {
                    foreach (var component in query.VisitComponents(current))
                    {
                        yield return component;
                    }

                    for (int i = 0; i < current.childCount; i++)
                    {
                        foreach (var component in TraverseRecursive(current.GetChild(i)))
                        {
                            yield return component;
                        }
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
            public IEnumerable<T> Traverse(HierarchyQuery<T> query)
            {
                return TraverseRecursive(query.Origin.transform);

                IEnumerable<T> TraverseRecursive(Transform current)
                {
                    if (current.parent != null)
                    {
                        foreach (var component in TraverseRecursive(current.parent))
                        {
                            yield return component;
                        }
                    }

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
                IEnumerable<Transform> sameLevelNodes;
                if (query.Origin.transform.parent == null)
                {
                    sameLevelNodes = Hierarchy.GetRootGameObject();
                }
                else
                {
                    // the Cast<> creates an IEnumerable from the parent's children
                    sameLevelNodes = query.Origin.transform.parent.Cast<Transform>();
                }

                foreach (Transform current in sameLevelNodes)
                {
                    foreach (var component in query.VisitComponents(current)) yield return component;
                }
            }
        }

        public class Compound : ITraversalAlgorithm<T>
        {
            private readonly List<ITraversalAlgorithm<T>> _traversals;
            public IReadOnlyCollection<ITraversalAlgorithm<T>> Traversals => _traversals;

            public Compound(params ITraversalAlgorithm<T>[] traversals) => _traversals = traversals.ToList();
            public void Add(ITraversalAlgorithm<T> traversal) => _traversals.Add(traversal);
            public void Insert(int index, ITraversalAlgorithm<T> traversal) => _traversals.Insert(index, traversal);
            public void Remove(ITraversalAlgorithm<T> traversal) => _traversals.Remove(traversal);
            public void Clear() => _traversals.Clear();

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
                var inScene = GameObject.FindObjectsOfType<T>(includeInactive: true); // is there a better way?  manually traverse from roots?
                foreach (var component in inScene)
                {
                    if (query.QueryTraversal.ShouldSkip(component.gameObject)) continue;
                    if (query.QueryTraversal.ShouldSkip(component)) continue;
                    if (query.Filter.MatchesFilters(query.Origin, component)) yield return component;
                }
            }
        }

        public class ImmediateChildren : ITraversalAlgorithm<T>
        {
            public IEnumerable<T> Traverse(HierarchyQuery<T> query)
            {
                var current = query.Origin.transform;
                for (int i = 0; i < current.childCount; i++)
                {
                    var child = current.GetChild(i);
                    foreach (var component in query.VisitComponents(child))
                    {
                        yield return component;
                    }
                }
            }
        }
    }
}