using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Toolbox
{
    public interface IQueryAlgorithm<T> where T : Component
    {
        IEnumerable<T> Traverse(Transform origin, IQueryFilter<T> queryFilter);
    }

    public enum EQueryAlgorithm
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

    public static class HierarchyQueryAlgorithm<T> where T : Component
    {
        private static readonly IReadOnlyDictionary<EQueryAlgorithm, Func<IQueryAlgorithm<T>>> TraversalAlgorithmFactories =
            new Dictionary<EQueryAlgorithm, Func<IQueryAlgorithm<T>>>
            {
                { EQueryAlgorithm.DescendantsBreadthFirst, () => new DescendantsBreadthFirst() },
                { EQueryAlgorithm.DescendantsDepthFirst, () => new DescendantsDepthFirst() },
                { EQueryAlgorithm.Ancestors, () => new Ancestors() },
                { EQueryAlgorithm.AncestorsTopDown, () => new AncestorsTopDown() },
                { EQueryAlgorithm.SameTreeLevel, () => new SameTreeLevel() },
                { EQueryAlgorithm.Compound, () => new Compound() },
                { EQueryAlgorithm.TraverseFindObjectsOfType, () => new SceneSearch() },
                { EQueryAlgorithm.ImmediateChildren, () => new ImmediateChildren() }
            };

        public static IQueryAlgorithm<T> Create(EQueryAlgorithm algorithm)
        {
            if (TraversalAlgorithmFactories.TryGetValue(algorithm, out var factory)) return factory();
            throw new ArgumentOutOfRangeException(nameof(algorithm), algorithm, "Unsupported traversal algorithm.");
        }

        public class DescendantsBreadthFirst : IQueryAlgorithm<T>
        {
            private readonly Queue<Transform> _queue = new();

            public IEnumerable<T> Traverse(Transform origin, IQueryFilter<T> queryFilter)
            {
                _queue.Clear();
                _queue.Enqueue(origin);
                while (_queue.Count > 0)
                {
                    var current = _queue.Dequeue();
                    foreach (var component in queryFilter.VisitComponentsOnGameObject(origin, current.gameObject))
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

        public class DescendantsDepthFirst : IQueryAlgorithm<T>
        {
            public IEnumerable<T> Traverse(Transform origin, IQueryFilter<T> queryFilter)
            {
                return TraverseRecursive(origin);

                IEnumerable<T> TraverseRecursive(Transform current)
                {
                    foreach (var component in queryFilter.VisitComponentsOnGameObject(origin, current.gameObject))
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

        public class Ancestors : IQueryAlgorithm<T>
        {
            public IEnumerable<T> Traverse(Transform origin, IQueryFilter<T> queryFilter)
            {
                var current = origin; // Start from the origin object so we can potentially include it
                while (current != null)
                {
                    foreach (var component in queryFilter.VisitComponentsOnGameObject(origin, current.gameObject)) yield return component;
                    current = current.parent;
                }
            }
        }

        public class AncestorsTopDown : IQueryAlgorithm<T>
        {
            public IEnumerable<T> Traverse(Transform origin, IQueryFilter<T> queryFilter)
            {
                return TraverseRecursive(origin);

                IEnumerable<T> TraverseRecursive(Transform current)
                {
                    if (current.parent != null)
                    {
                        foreach (var component in TraverseRecursive(current.parent))
                        {
                            yield return component;
                        }
                    }

                    foreach (var component in queryFilter.VisitComponentsOnGameObject(origin, current.gameObject))
                    {
                        yield return component;
                    }
                }
            }
        }

        public class SameTreeLevel : IQueryAlgorithm<T>
        {
            public IEnumerable<T> Traverse(Transform origin, IQueryFilter<T> queryFilter)
            {
                IEnumerable<Transform> sameLevelNodes;
                if (origin.parent == null)
                {
                    sameLevelNodes = Hierarchy.GetRootGameObject();
                }
                else
                {
                    // the Cast<> creates an IEnumerable from the parent's children
                    sameLevelNodes = Enumerable.Cast<Transform>(origin.parent);
                }

                foreach (Transform current in sameLevelNodes)
                {
                    foreach (var component in queryFilter.VisitComponentsOnGameObject(origin, current.gameObject)) yield return component;
                }
            }
        }

        public class Compound : IQueryAlgorithm<T>
        {
            private readonly List<IQueryAlgorithm<T>> _traversals;
            public IReadOnlyCollection<IQueryAlgorithm<T>> Traversals => _traversals;

            public Compound(params IQueryAlgorithm<T>[] traversals) => _traversals = traversals.ToList();
            public void Add(IQueryAlgorithm<T> query) => _traversals.Add(query);
            public void Insert(int index, IQueryAlgorithm<T> query) => _traversals.Insert(index, query);
            public void Remove(IQueryAlgorithm<T> query) => _traversals.Remove(query);
            public void Clear() => _traversals.Clear();

            public IEnumerable<T> Traverse(Transform origin, IQueryFilter<T> queryFilter)
            {
                foreach (var traversal in _traversals)
                {
                    foreach (var component in traversal.Traverse(origin, queryFilter)) yield return component;
                }
            }
        }

        public class SceneSearch : IQueryAlgorithm<T>
        {
            public IEnumerable<T> Traverse(Transform origin, IQueryFilter<T> queryFilter)
            {
                var componentsInScene = GameObject.FindObjectsOfType<T>(includeInactive: true); // is there a better way?  manually traverse from roots?
                foreach (var componentInScene in componentsInScene)
                {
                    foreach (var filteredComponent in queryFilter.VisitComponent(origin, componentInScene))
                    {
                        yield return filteredComponent;
                    }
                }
            }
        }

        public class ImmediateChildren : IQueryAlgorithm<T>
        {
            public IEnumerable<T> Traverse(Transform origin, IQueryFilter<T> queryFilter)
            {
                var current = origin;
                for (int i = 0; i < current.childCount; i++)
                {
                    var child = current.GetChild(i);
                    foreach (var component in queryFilter.VisitComponentsOnGameObject(origin, child.gameObject))
                    {
                        yield return component;
                    }
                }
            }
        }
    }
}