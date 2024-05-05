using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Toolbox
{
    public static class ComponentHierarchyQueryExtensions
    {
        public static ComponentHierarchyQuery<T> GetAncestors<T>(this Component component) where T : Component
        {
            return component.HierarchyQuery<T>().WithTraversal(a => a.TraversalAlgorithm = HierarchyTraversalAlgorithms<T>.Get(ETraversalAlgorithm.Ancestors));
        }

        public static ComponentHierarchyQuery<T> GetDescendants<T>(this Component component) where T : Component
        {
            return component.HierarchyQuery<T>().WithTraversal(a => a.TraversalAlgorithm = HierarchyTraversalAlgorithms<T>.Get(ETraversalAlgorithm.DescendantsBreadthFirst));
        }

        public static ComponentHierarchyQuery<T> GetScene<T>(this Component component) where T : Component
        {
            return component.HierarchyQuery<T>().WithTraversal(a => a.TraversalAlgorithm = HierarchyTraversalAlgorithms<T>.Get(ETraversalAlgorithm.EntireScene));
        }

        public static ComponentHierarchyQuery<T> HierarchyQuery<T>(this Component component) where T : Component => new(component);
    }

    public enum ETraversalAlgorithm
    {
        DescendantsBreadthFirst,
        DescendantsDepthFirst,
        Ancestors,
        SameTreeLevel,
        EntireScene
    }

    public static class HierarchyTraversalAlgorithms<T> where T : Component
    {
        public static TraversalAlgorithm<T> Get(ETraversalAlgorithm algorithm)
        {
            return algorithm switch
            {
                ETraversalAlgorithm.DescendantsBreadthFirst => TraverseDescendantsBreadthFirst,
                ETraversalAlgorithm.DescendantsDepthFirst => TraverseDescendantsDepthFirst,
                ETraversalAlgorithm.Ancestors => TraverseAncestors,
                ETraversalAlgorithm.SameTreeLevel => TraverseSameTreeLevel,
                ETraversalAlgorithm.EntireScene => TraverseEntireScene,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public static IEnumerable<T> TraverseDescendantsBreadthFirst(ComponentHierarchyQuery<T> query)
        {
            var queue = new Queue<Transform>();
            queue.Enqueue(query.Origin.transform); // Start from the origin object so we can potentially include it
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (query.Traversal.ShouldHaltTraversal(current)) continue;
                foreach (Transform child in current) queue.Enqueue(child);
                foreach (var component in VisitComponents(current, query)) yield return component;
            }
        }

        public static IEnumerable<T> TraverseDescendantsDepthFirst(ComponentHierarchyQuery<T> query)
        {
            var stack = new Stack<Transform>();
            stack.Push(query.Origin.transform); // Start from the origin object so we can potentially include it
            while (stack.Count > 0)
            {
                var current = stack.Pop();
                if (query.Traversal.ShouldHaltTraversal(current)) continue;
                foreach (var component in VisitComponents(current, query)) yield return component;
                for (int i = current.childCount - 1; i >= 0; i--) stack.Push(current.GetChild(i));
            }
        }

        public static IEnumerable<T> TraverseAncestors(ComponentHierarchyQuery<T> query)
        {
            var current = query.Origin.transform; // Start from the origin object so we can potentially include it
            while (current != null)
            {
                if (query.Traversal.ShouldHaltTraversal(current)) yield break;
                foreach (var component in VisitComponents(current, query)) yield return component;
                current = current.parent;
            }
        }

        public static IEnumerable<T> TraverseSameTreeLevel(ComponentHierarchyQuery<T> query)
        {
            var sameLevel = query.Origin.transform.parent != null ? query.Origin.transform.parent.Cast<Transform>() : Hierarchy.GetRootGameObject();
            foreach (Transform current in sameLevel)
            {
                if (query.Traversal.ShouldHaltTraversal(current)) continue; // Skip objects based on custom criteria
                foreach (var component in VisitComponents(current, query)) yield return component;
            }
        }

        public static IEnumerable<T> TraverseEntireScene(ComponentHierarchyQuery<T> query)
        {
            foreach (var component in GameObject.FindObjectsOfType<T>())
            {
                if (query.Traversal.ShouldSkip(component.gameObject)) continue;
                if (query.Traversal.ShouldSkip(component)) continue;
                if (query.Filter.MatchesFilters(query.Origin, component)) yield return component;
            }
        }

        public static IEnumerable<T> VisitComponents(Transform transform, ComponentHierarchyQuery<T> query)
        {
            if (query.Traversal.ShouldSkip(transform.gameObject)) yield break;

            foreach (var visit in transform.GetComponents<T>())
            {
                if (query.Traversal.ShouldSkip(visit)) continue;
                if (query.Filter.MatchesFilters(query.Origin, visit)) yield return visit;
            }
        }
    }

    [Flags]
    public enum InactiveTraversal
    {
        IncludeInactive = 0,
        SkipInactiveGameObjects = 1,
        SkipInactiveComponents = 2,
        HaltInactiveGameObjects = 4,
        HaltInactiveComponents = 8,
        SkipAllInactive = SkipInactiveComponents | SkipInactiveGameObjects,
        HaltAllInactive = HaltInactiveComponents | HaltInactiveGameObjects
    }

    public class HierarchyTraversalRules<T> where T : Component
    {
        public TraversalAlgorithm<T> TraversalAlgorithm { get; set; }
        public InactiveTraversal InactiveTraversal { get; set; } = InactiveTraversal.SkipAllInactive;

        public bool ShouldSkip(GameObject gameObject) => !gameObject.activeInHierarchy && InactiveTraversal.HasFlag(InactiveTraversal.SkipInactiveGameObjects);

        public bool ShouldSkip(T component)
        {
            if (component is Behaviour behaviour && !behaviour.enabled) return InactiveTraversal.HasFlag(InactiveTraversal.SkipInactiveComponents);
            return false;
        }

        public bool ShouldHaltTraversal(Transform transform) => !transform.gameObject.activeInHierarchy && InactiveTraversal.HasFlag(InactiveTraversal.HaltInactiveGameObjects);
    }

    public class ComponentFilterRules<T> where T : Component
    {
        public bool AllowOriginComponent { get; set; } = false;
        public bool AllowOriginGameObject { get; set; } = false;
        public string NameIs { get; set; }
        public string NameContains { get; set; }
        public Regex NameRegex { get; set; }
        public Func<T, bool> CustomFilter { get; set; }

        public bool MatchesFilters(Component origin, T component)
        {
            if (origin == component && !AllowOriginComponent) return false;
            if (origin.gameObject == component.gameObject && !AllowOriginGameObject) return false;
            if (!string.IsNullOrEmpty(NameIs) && component.name != NameIs) return false;
            if (!string.IsNullOrEmpty(NameContains) && !component.name.Contains(NameContains)) return false;
            if (NameRegex != null && !NameRegex.IsMatch(component.name)) return false;
            if (CustomFilter != null && !CustomFilter(component)) return false;
            return true;
        }
    }

    public delegate IEnumerable<T> TraversalAlgorithm<T>(ComponentHierarchyQuery<T> query) where T : Component;

    public class ComponentHierarchyQuery<T> : IEnumerable<T> where T : Component
    {
        public Component Origin { get; }
        public HierarchyTraversalRules<T> Traversal { get; set; } = new HierarchyTraversalRules<T>();
        public ComponentFilterRules<T> Filter { get; set; } = new ComponentFilterRules<T>();
        public ComponentHierarchyQuery(Component origin)
        {
            Origin = origin;
        }

        public ComponentHierarchyQuery<T> WithTraversal(Action<HierarchyTraversalRules<T>> action)
        {
            action(Traversal);
            return this;
        }

        public ComponentHierarchyQuery<T> WithTraversal(HierarchyTraversalRules<T> rules)
        {
            Traversal = rules;
            return this;
        }

        public ComponentHierarchyQuery<T> WithFilter(Action<ComponentFilterRules<T>> action)
        {
            action(Filter);
            return this;
        }

        public ComponentHierarchyQuery<T> WithFilter(ComponentFilterRules<T> rules)
        {
            Filter = rules;
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<T> GetEnumerator() => SelectEnumerable().GetEnumerator();
        private IEnumerable<T> SelectEnumerable() => Traversal.TraversalAlgorithm(this);
    }
}