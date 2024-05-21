using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Toolbox
{
    public enum EHierarchyQueryAlgorithm
    {
        None,
        SameGameObject,

        ImmediateDescendants,
        DescendantsBreadthFirst,
        DescendantsDepthFirst,

        ImmediateAncestor,
        Ancestors,
        AncestorsTopDown,

        SameTreeLevel,
        EntireScene,

        Compound,

        // IntersectsRay,
        // IntersectsFrustum,
        // IntersectsSphere,
        // IntersectsBox,
        // FindNearestNeighbors,
        // FindObjectsInRange,
    }

    public static class HierarchyQueryAlgorithm<T> where T : Component
    {
        private static readonly IReadOnlyDictionary<EHierarchyQueryAlgorithm, Func<IQueryAlgorithm<T>>> Factories = new Dictionary<EHierarchyQueryAlgorithm, Func<IQueryAlgorithm<T>>>
        {
            { EHierarchyQueryAlgorithm.None, () => EmptyQueryAlgorithm<T>.Empty },
            { EHierarchyQueryAlgorithm.SameGameObject, () => new SameGameObject() },

            { EHierarchyQueryAlgorithm.ImmediateDescendants, () => new ImmediateDescendants() },
            { EHierarchyQueryAlgorithm.DescendantsBreadthFirst, () => new DescendantsBreadthFirst() },
            { EHierarchyQueryAlgorithm.DescendantsDepthFirst, () => new DescendantsDepthFirst() },

            { EHierarchyQueryAlgorithm.ImmediateAncestor, () => new ImmediateAncestor() },
            { EHierarchyQueryAlgorithm.Ancestors, () => new Ancestors() },
            { EHierarchyQueryAlgorithm.AncestorsTopDown, () => new AncestorsTopDown() },

            { EHierarchyQueryAlgorithm.SameTreeLevel, () => new SameTreeLevel() },
            { EHierarchyQueryAlgorithm.EntireScene, () => new EntireScene() },
            { EHierarchyQueryAlgorithm.Compound, () => new Compound() },
        };

        public static IQueryAlgorithm<T> Create(EHierarchyQueryAlgorithm algorithm)
        {
            if (Factories.TryGetValue(algorithm, out var factory)) return factory();
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

            public Compound(params IQueryAlgorithm<T>[] traversals) : this(traversals.ToList())
            {
            }

            public Compound(IEnumerable<IQueryAlgorithm<T>> traversals) : this(traversals.ToList())
            {
            }

            public Compound(List<IQueryAlgorithm<T>> traversals) => _traversals = traversals;

            public void Add(IQueryAlgorithm<T> query) => _traversals.Add(query);
            public void Insert(int index, IQueryAlgorithm<T> query) => _traversals.Insert(index, query);
            public void Remove(IQueryAlgorithm<T> query) => _traversals.Remove(query);
            public void Clear() => _traversals.Clear();

            public IEnumerable<T> Traverse(Transform origin, IQueryFilter<T> queryFilter)
            {
                // todo somehow sort these tarversals first?
                foreach (var traversal in _traversals)
                {
                    foreach (var component in traversal.Traverse(origin, queryFilter)) yield return component;
                }
            }
        }

        public class EntireScene : IQueryAlgorithm<T>
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

        public class SameGameObject : IQueryAlgorithm<T>
        {
            public IEnumerable<T> Traverse(Transform origin, IQueryFilter<T> queryFilter)
            {
                foreach (var component in queryFilter.VisitComponentsOnGameObject(origin, origin.gameObject))
                {
                    yield return component;
                }
            }
        }

        public class ImmediateDescendants : IQueryAlgorithm<T>
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

        public class ImmediateAncestor : IQueryAlgorithm<T>
        {
            public IEnumerable<T> Traverse(Transform origin, IQueryFilter<T> queryFilter)
            {
                var current = origin; // Start from the origin object, so we can potentially include it

                foreach (var component in queryFilter.VisitComponentsOnGameObject(origin, current.gameObject))
                {
                    yield return component;
                }

                if (current.parent == null) yield break;
                foreach (var component in queryFilter.VisitComponentsOnGameObject(origin, current.parent.gameObject))
                {
                    yield return component;
                }
            }
        }
    }
}
